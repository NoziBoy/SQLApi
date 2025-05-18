using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using SQLApi.Models;
using System.Linq;

namespace SQLApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly VentaContext _context;

        public TicketsController(VentaContext context)
        {
            _context = context;
        }

        [HttpGet("resumen-alumnos")]
        public ActionResult<IEnumerable<AlumnoResumenDTO>> GetResumenAlumnos()
        {
            var resumen = _context.Alumnos
                .Select(al => new AlumnoResumenDTO
                {
                    AlumnoID = al.AlumnoId, 
                    Matricula = al.Matricula,
                    NombreCompleto = al.Nombres + " " + al.Apellidos,
                    Grado = al.Grado,
                    Grupo = al.Grupo,
                    TelefonoTutor = al.TelefonoTutor,
                    Categoria = al.Categoria,
                    SaldoPendienteTotal = al.Tickets.Sum(t => (decimal?)t.SaldoPendiente) ?? 0,
                    TotalAbonado = al.Tickets.Sum(t => (decimal?)t.TotalPagado) ?? 0,
                    TotalTickets = al.Tickets.Count()
                })
                .ToList();

            return Ok(resumen);
        }

        [HttpGet("detalle-alumno-id")]
        public ActionResult<IEnumerable<DetalleTicketAlumnoDTO>> GetDetalleTicketsPorId(int id)
        {
            var tickets = _context.Tickets
                .Where(t => t.AlumnoId == id)
                .Select(t => new DetalleTicketAlumnoDTO
                {
                    NumeroTicket = t.TicketId,
                    AlumnoID = t.AlumnoId,
                    TotalTicket = (decimal)(t.Total ?? 0),
                    TotalAbonado = (decimal)(t.TotalPagado ?? 0),
                    SaldoPendiente = (decimal)(t.SaldoPendiente ?? 0),
                    EstadoTicket = t.Estado,
                    EstadoPago = (t.TotalPagado ?? 0) >= (t.Total ?? 0)
                                    ? "Pagado"
                                    : ((t.TotalPagado ?? 0) > 0 ? "Abono" : "Pendiente"),
                    FechaCreacion = t.FechaApertura ?? DateTime.MinValue
                })
                .OrderByDescending(t => t.FechaCreacion)
                .ToList();

            return Ok(tickets);
        }



        [HttpGet("generar-pdf")]
        public IActionResult GenerarPDF(int ticketId)
        {
            var ticket = _context.Tickets
                .Where(t => t.TicketId == ticketId)
                .Select(t => new
                {
                    t.TicketId,
                    t.FechaApertura,
                    t.Total,
                    t.TotalPagado,
                    t.SaldoPendiente,
                    t.Estado,
                    Alumno = new
                    {
                        t.Alumno.Nombres,
                        t.Alumno.Apellidos,
                        t.Alumno.Matricula
                    },
                    Detalles = t.DetalleTickets.Select(d => new
                    {
                        d.FechaRegistro,
                        d.Producto,
                        d.Cantidad,
                        d.Precio
                    }).ToList(),
                    Abonos = t.Abonos.Select(a => new
                    {
                        a.FechaAbono,
                        a.Monto,
                        a.Observaciones
                    }).ToList(),
                    Empresa = _context.Empresas.FirstOrDefault()
                })
                .FirstOrDefault();

            if (ticket == null)
                return NotFound("Ticket no encontrado");

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                BaseColor colorPrimario = new BaseColor(47, 54, 64);
                BaseColor colorSecundario = new BaseColor(70, 80, 95);
                BaseColor colorFondoTabla = new BaseColor(245, 246, 250);
                BaseColor colorResaltado = new BaseColor(52, 172, 224);

                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font fuenteTitulo = new Font(baseFont, 18, Font.BOLD, colorPrimario);
                Font fuenteSubtitulo = new Font(baseFont, 14, Font.BOLD, colorPrimario);
                Font fuenteNormal = new Font(baseFont, 12, Font.NORMAL, colorPrimario);
                Font fuenteEncabezado = new Font(baseFont, 12, Font.BOLD, BaseColor.WHITE);

                PdfPTable tablaEmpresa = new PdfPTable(2);
                tablaEmpresa.WidthPercentage = 100;
                tablaEmpresa.SetWidths(new float[] { 1f, 3f });

                if (ticket.Empresa?.Logo != null)
                {
                    using (MemoryStream msLogo = new MemoryStream(ticket.Empresa.Logo))
                    {
                        Image logo = Image.GetInstance(msLogo);
                        logo.ScaleToFit(120, 120);
                        PdfPCell logoCell = new PdfPCell(logo) { Border = 0 };
                        tablaEmpresa.AddCell(logoCell);
                    }
                }
                else
                {
                    tablaEmpresa.AddCell(new PdfPCell() { Border = 0 });
                }

                PdfPCell datosEmpresa = new PdfPCell() { Border = 0 };
                datosEmpresa.AddElement(new Paragraph(ticket.Empresa?.Nombre ?? "", fuenteTitulo));
                datosEmpresa.AddElement(new Paragraph("Dirección: " + ticket.Empresa?.Direccion, fuenteNormal));
                datosEmpresa.AddElement(new Paragraph("Correo: " + ticket.Empresa?.Correo, fuenteNormal));
                datosEmpresa.AddElement(new Paragraph("Teléfono: " + ticket.Empresa?.Telefono, fuenteNormal));
                tablaEmpresa.AddCell(datosEmpresa);

                doc.Add(tablaEmpresa);
                doc.Add(new Paragraph("\n"));

                PdfPTable tablaAlumno = new PdfPTable(2);
                tablaAlumno.WidthPercentage = 100;
                tablaAlumno.SetWidths(new float[] { 1f, 3f });
                tablaAlumno.AddCell(new PdfPCell(new Paragraph("Alumno:", fuenteSubtitulo)) { Border = 0 });
                tablaAlumno.AddCell(new PdfPCell(new Paragraph(ticket.Alumno.Nombres + " " + ticket.Alumno.Apellidos, fuenteNormal)) { Border = 0 });
                tablaAlumno.AddCell(new PdfPCell(new Paragraph("Matrícula:", fuenteSubtitulo)) { Border = 0 });
                tablaAlumno.AddCell(new PdfPCell(new Paragraph(ticket.Alumno.Matricula, fuenteNormal)) { Border = 0 });
                doc.Add(tablaAlumno);
                doc.Add(new Paragraph("\n"));

                PdfPTable tablaDetalles = new PdfPTable(5);
                tablaDetalles.WidthPercentage = 100;
                tablaDetalles.SetWidths(new float[] { 1f, 2f, 1f, 1f, 1f });

                tablaDetalles.AddCell(new PdfPCell(new Paragraph("Fecha", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                tablaDetalles.AddCell(new PdfPCell(new Paragraph("Producto", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                tablaDetalles.AddCell(new PdfPCell(new Paragraph("Cantidad", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                tablaDetalles.AddCell(new PdfPCell(new Paragraph("Precio", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                tablaDetalles.AddCell(new PdfPCell(new Paragraph("Subtotal", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });

                int fila = 0;
                foreach (var d in ticket.Detalles)
                {
                    BaseColor colorFila = (fila % 2 == 0) ? BaseColor.WHITE : colorFondoTabla;
                    tablaDetalles.AddCell(new PdfPCell(new Paragraph(Convert.ToDateTime(d.FechaRegistro).ToString("dd/MM/yyyy"), fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_CENTER });
                    tablaDetalles.AddCell(new PdfPCell(new Paragraph(d.Producto, fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_LEFT });
                    tablaDetalles.AddCell(new PdfPCell(new Paragraph(d.Cantidad.ToString(), fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_CENTER });
                    tablaDetalles.AddCell(new PdfPCell(new Paragraph(d.Precio.ToString("C2"), fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_RIGHT });
                    tablaDetalles.AddCell(new PdfPCell(new Paragraph((d.Cantidad * d.Precio).ToString("C2"), fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_RIGHT });
                    fila++;
                }
                doc.Add(tablaDetalles);
                doc.Add(new Paragraph("\n"));

                PdfPTable tablaTotales = new PdfPTable(2);
                tablaTotales.WidthPercentage = 60;
                tablaTotales.HorizontalAlignment = Element.ALIGN_RIGHT;
                tablaTotales.SetWidths(new float[] { 1f, 1f });
                Font fuenteTituloTotal = new Font(baseFont, 14, Font.BOLD, colorResaltado);
                Font fuenteTotalRojo = new Font(baseFont, 16, Font.BOLD, BaseColor.RED);


                tablaTotales.AddCell(new PdfPCell(new Paragraph("Total:", fuenteSubtitulo)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                tablaTotales.AddCell(new PdfPCell(new Paragraph(ticket.Total?.ToString("C2"), fuenteTotalRojo)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                tablaTotales.AddCell(new PdfPCell(new Paragraph("Pagado:", fuenteSubtitulo)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                tablaTotales.AddCell(new PdfPCell(new Paragraph(ticket.TotalPagado?.ToString("C2"), fuenteNormal)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                tablaTotales.AddCell(new PdfPCell(new Paragraph("Saldo Pendiente:", fuenteSubtitulo)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
                tablaTotales.AddCell(new PdfPCell(new Paragraph(ticket.SaldoPendiente?.ToString("C2"), fuenteNormal)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });

                doc.Add(tablaTotales);

                if (ticket.Abonos.Any())
                {
                    doc.Add(new Paragraph("\nAbonos:", fuenteSubtitulo));
                    PdfPTable tablaAbonos = new PdfPTable(3);
                    tablaAbonos.WidthPercentage = 100;
                    tablaAbonos.SetWidths(new float[] { 1f, 1f, 1f });

                    tablaAbonos.AddCell(new PdfPCell(new Paragraph("Fecha", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                    tablaAbonos.AddCell(new PdfPCell(new Paragraph("Monto Abonado", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });
                    tablaAbonos.AddCell(new PdfPCell(new Paragraph("Observaciones", fuenteEncabezado)) { BackgroundColor = colorSecundario, HorizontalAlignment = Element.ALIGN_CENTER });

                    fila = 0;
                    foreach (var ab in ticket.Abonos)
                    {
                        BaseColor colorFila = (fila % 2 == 0) ? BaseColor.WHITE : colorFondoTabla;
                        tablaAbonos.AddCell(new PdfPCell(new Paragraph(ab.FechaAbono?.ToString("dd/MM/yyyy") ?? "-", fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_CENTER });
                        tablaAbonos.AddCell(new PdfPCell(new Paragraph(ab.Monto.ToString("C2"), fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_RIGHT });
                        tablaAbonos.AddCell(new PdfPCell(new Paragraph(ab.Observaciones ?? "", fuenteNormal)) { BackgroundColor = colorFila, HorizontalAlignment = Element.ALIGN_LEFT });
                        fila++;
                    }
                    doc.Add(tablaAbonos);
                }

                doc.Close();
                return File(ms.ToArray(), "application/pdf", $"Ticket_{ticket.TicketId}.pdf");
            }
        }




    }
}
