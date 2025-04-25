using Gestion_Biblioteca.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Gestion_Biblioteca.Controllers
{
    public class PrestamoController : Controller
    {
        private readonly PrestamoService _prestamoService;
        private readonly UsuarioService _usuarioService;
        private readonly LibroService _libroService;

        public PrestamoController(PrestamoService prestamoService, UsuarioService usuarioService, LibroService libroService)
        {
            _prestamoService = prestamoService;
            _usuarioService = usuarioService;
            _libroService = libroService;
        }

        public async Task<IActionResult> Index()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();
            return View(prestamos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prestamos prestamo)
        {
           
            Console.WriteLine(JsonConvert.SerializeObject(prestamo));

            if (ModelState.IsValid)
            {
                try
                {
                    var nuevoPrestamo = await _prestamoService.CreatePrestamoAsync(prestamo);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {                 
                    ModelState.AddModelError(string.Empty, "Error al registrar el préstamo. Intenta nuevamente.");
                }
            }

            await CargarListas();
            return View(prestamo);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null) return NotFound();

            await CargarListas();
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prestamos prestamo)
        {
            if (id != prestamo.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _prestamoService.UpdatePrestamoAsync(id, prestamo);
                return RedirectToAction(nameof(Index));
            }

            await CargarListas();
            return View(prestamo);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var prestamo = await _prestamoService.GetPrestamoByIdAsync(id);
            if (prestamo == null) return NotFound();
            return View(prestamo);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _prestamoService.DeletePrestamoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            var libros = await _libroService.GetAllLibrosAsync();

            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");
            ViewBag.Libros = new SelectList(libros, "Id", "Titulo");
        }

       public async Task<IActionResult> ExportToPdf()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync(); // Asegúrate que incluya nombres de usuario/libro

            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();

                doc.Add(new Paragraph("Lista de Préstamos"));
                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.AddCell("Usuario");
                table.AddCell("Libro");
                table.AddCell("Fecha Préstamo");
                table.AddCell("Fecha Devolución Esperada");
                table.AddCell("Fecha Devolución Real");
                table.AddCell("Estado");

                foreach (var p in prestamos)
                {
                    table.AddCell(p.IdUsuario.ToString()); // Asume que ya lo trae
                    table.AddCell(p.IdLibro.ToString());
                    table.AddCell(p.FechaPrestamo.ToString("dd/MM/yyyy"));
                    table.AddCell(p.FechaDevolucionEsperada.ToString("dd/MM/yyyy"));
                    table.AddCell(p.FechaDevolucionReal?.ToString("dd/MM/yyyy") ?? ""); // Si es null
                    table.AddCell(p.Estado);
                }

                doc.Add(table);
                doc.Close();
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", "Prestamos.pdf");
            }
        }


        public async Task<IActionResult> ExportToExcel()
        {
            var prestamos = await _prestamoService.GetAllPrestamosAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Préstamos");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Usuario";
                worksheet.Cells[1, 2].Value = "Libro";
                worksheet.Cells[1, 3].Value = "Fecha Préstamo";
                worksheet.Cells[1, 4].Value = "Fecha Devolución Esperada";
                worksheet.Cells[1, 5].Value = "Fecha Devolución Real";
                worksheet.Cells[1, 6].Value = "Estado";

                int row = 2;
                foreach (var p in prestamos)
                {
                    worksheet.Cells[row, 1].Value = p.IdUsuario;
                    worksheet.Cells[row, 2].Value = p.IdLibro;
                    worksheet.Cells[row, 3].Value = p.FechaPrestamo;
                    worksheet.Cells[row, 4].Value = p.FechaDevolucionEsperada;
                    worksheet.Cells[row, 5].Value = p.FechaDevolucionReal;
                    worksheet.Cells[row, 6].Value = p.Estado;

                    worksheet.Cells[row, 3].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "dd/mm/yyyy";

                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos.xlsx");
            }
        }





    }
}
