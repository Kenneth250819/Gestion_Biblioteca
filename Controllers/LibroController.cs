﻿using Gestion_Biblioteca.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Gestion_Biblioteca.Controllers
{
        public class LibroController : Controller
        {
            private readonly LibroService _libroService;

            public LibroController(LibroService libroService)
            {
                _libroService = libroService;
            }

        public async Task<IActionResult> Index(string? titulo, string? autor)
        {
            List<Libros> libros;

            if (!string.IsNullOrWhiteSpace(titulo) || !string.IsNullOrWhiteSpace(autor))
            {
                libros = await _libroService.BuscarLibrosAsync(titulo, autor);
            }
            else
            {
                libros = (await _libroService.GetAllLibrosAsync()).ToList();
            }

            ViewBag.Titulo = titulo;
            ViewBag.Autor = autor;

            return View(libros);
        }



        // GET: /Libro/Details/5
        public async Task<IActionResult> Details(int id)
            {
                var libro = await _libroService.GetLibroByIdAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }

            // GET: /Libro/Create
            public IActionResult Create()
            {
                return View();
            }


            // POST: /Libro/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Libros libro)
            {
                if (ModelState.IsValid)
                {
                    await _libroService.CreateLibroAsync(libro);
                    return RedirectToAction(nameof(Index));
                }
                return View(libro);
            }

            // GET: /Libro/Edit/5
            public async Task<IActionResult> Edit(int id)
            {
                var libro = await _libroService.GetLibroByIdAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }

            // POST: /Libro/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, Libros libro)
            {
                if (id != libro.Id)
                {
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    await _libroService.UpdateLibroAsync(id, libro);
                    return RedirectToAction(nameof(Index));
                }
                return View(libro);
            }

            // GET: /Libro/Delete/5
            public async Task<IActionResult> Delete(int id)
            {
                var libro = await _libroService.GetLibroByIdAsync(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }

            // POST: /Libro/Delete/5
            [HttpPost, ActionName("DeleteConfirmed")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _libroService.DeleteLibroAsync(id);
                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> ExportToPdf()
            {
                var libros = await _libroService.GetAllLibrosAsync();

                using (var stream = new MemoryStream())
                {
                    var doc = new iTextSharp.text.Document();
                    PdfWriter.GetInstance(doc, stream).CloseStream = false;
                    doc.Open();

                    doc.Add(new iTextSharp.text.Paragraph("Lista de Libros"));
                    doc.Add(new iTextSharp.text.Paragraph(" "));

                    var table = new PdfPTable(7);
                    table.AddCell("Título");
                    table.AddCell("Autor");
                    table.AddCell("Editorial");
                    table.AddCell("ISBN");
                    table.AddCell("Año");
                    table.AddCell("Categoría");
                    table.AddCell("Existencias");

                    foreach (var libro in libros)
                    {
                        table.AddCell(libro.Titulo);
                        table.AddCell(libro.Autor);
                        table.AddCell(libro.Editorial);
                        table.AddCell(libro.ISBN);
                        table.AddCell(libro.Anio.ToString());
                        table.AddCell(libro.Categoria);
                        table.AddCell(libro.Existencias.ToString());
                    }

                    doc.Add(table);
                    doc.Close();

                    stream.Position = 0;
                    return File(stream.ToArray(), "application/pdf", "Libros.pdf");
                }
            }

            public async Task<IActionResult> ExportToExcel()
            {

                var libros = await _libroService.GetAllLibrosAsync();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Libros");

                    // Encabezados
                    worksheet.Cells[1, 1].Value = "Título";
                    worksheet.Cells[1, 2].Value = "Autor";
                    worksheet.Cells[1, 3].Value = "Editorial";
                    worksheet.Cells[1, 4].Value = "ISBN";
                    worksheet.Cells[1, 5].Value = "Año";
                    worksheet.Cells[1, 6].Value = "Categoría";
                    worksheet.Cells[1, 7].Value = "Existencias";

                    int row = 2;
                    foreach (var libro in libros)
                    {
                        worksheet.Cells[row, 1].Value = libro.Titulo;
                        worksheet.Cells[row, 2].Value = libro.Autor;
                        worksheet.Cells[row, 3].Value = libro.Editorial;
                        worksheet.Cells[row, 4].Value = libro.ISBN;
                        worksheet.Cells[row, 5].Value = libro.Anio;
                        worksheet.Cells[row, 6].Value = libro.Categoria;
                        worksheet.Cells[row, 7].Value = libro.Existencias;
                        row++;
                    }

                    var stream = new MemoryStream(package.GetAsByteArray());
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Libros.xlsx");
                }
            }

        }
}
