using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Apollo.Core.Services.Common
{
    public class PdfService : IPdfService
    {
        #region Constants

        private int LEFT = 0;
        private int CENTRE = 1;
        private int RIGHT = 2;

        #endregion

        #region Fields

        private readonly MediaSettings _mediaSettings;
        private readonly TaxSettings _taxSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public PdfService(
            MediaSettings mediaSettings,
            TaxSettings taxSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _mediaSettings = mediaSettings;
            _taxSettings = taxSettings;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion

        public byte[] PrintBranchPendingLinePdf(IList<BranchPendingLineDownload> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                PrintBranchPendingLinePdf(stream, list);
                bytes = stream.ToArray();
            }

            return bytes;
        }

        public byte[] PrintOrderToInvoicePdf(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                PrintOrderToInvoicePdf(stream, order);
                bytes = stream.ToArray();
            }

            return bytes;
        }

        public byte[] PrintInventoryPendingLinePdf(IList<InventoryPendingLine> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);

                doc.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
                Font itemTitle = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
                Font itemFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f);
                Chunk titleText = new Chunk("Pending Lines", titleFont);
                LineSeparator firstPageUnderLine = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -5f);
                
                doc.Add(titleText);
                doc.Add(firstPageUnderLine);

                // Items
                doc.Add(GenerateItems(itemTitle, itemFont, list));

                doc.Close();

                bytes = stream.ToArray();
            }

            return bytes;
        }

        public byte[] PrintPickInProgressLinePdf(IList<PickingLineItem> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);

                doc.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
                Font itemTitle = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
                Font itemFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f);
                Chunk titleText = new Chunk("Pick In Progress", titleFont);
                LineSeparator firstPageUnderLine = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -5f);

                doc.Add(titleText);
                doc.Add(firstPageUnderLine);

                // Items
                doc.Add(GenerateItems(itemTitle, itemFont, list));

                doc.Close();

                bytes = stream.ToArray();
            }

            return bytes;
        }

        public void PrintOrderToInvoicePdf(Stream stream, IList<Order> orders)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (orders == null) throw new ArgumentNullException("orders");

            var pageSize = PageSize.A4;

            //if (_pdfSettings.LetterPageSizeEnabled)
            //{
            //    pageSize = PageSize.LETTER;
            //}

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            int ordCount = orders.Count;
            int ordNum = 0;
            var logoUrl = _mediaSettings.LargeLogoLink;
            var companyName = _storeInformationSettings.CompanyName;

            foreach (var order in orders)
            {
                #region Header

                //header
                var headerTable = new PdfPTable(2);
                headerTable.SetWidths(new[] { 0.5f, 0.5f });
                headerTable.WidthPercentage = 100f;
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                var cellHeader = new PdfPTable(1);
                cellHeader.DefaultCell.Border = Rectangle.NO_BORDER;
                cellHeader.AddCell(new Paragraph("Bill from:", titleFont));
                cellHeader.AddCell(new Paragraph(companyName, font));

                //TODO: Move to settings
                cellHeader.AddCell(new Paragraph("Unit 27, Orbital 25 Business Park", font));
                cellHeader.AddCell(new Paragraph("Dwight Road", font));
                cellHeader.AddCell(new Paragraph("Watford", font));
                cellHeader.AddCell(new Paragraph("Herts WD18 9DA", font));
                cellHeader.AddCell(new Paragraph("United Kingdom", font));
                cellHeader.AddCell(new Paragraph("VAT No. 921418642", font));
                cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                headerTable.AddCell(cellHeader);

                //logo and invoice header
                var cellLogoInvoice = new PdfPTable(1);
                cellLogoInvoice.DefaultCell.Border = Rectangle.NO_BORDER;

                var logo = Image.GetInstance(new Uri(logoUrl));
                logo.ScalePercent(70f);
                var cellLogo = new PdfPCell(logo);
                cellLogo.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellLogo.Border = Rectangle.NO_BORDER;
                cellLogoInvoice.AddCell(cellLogo);

                var cellInvoiceHeader = new PdfPTable(2);
                cellInvoiceHeader.SetWidths(new[] { 0.4f, 0.6f });
                cellInvoiceHeader.DefaultCell.Border = Rectangle.NO_BORDER;

                var cellEmptyField = new PdfPCell(new Phrase(""));
                cellEmptyField.Border = Rectangle.NO_BORDER;
                cellInvoiceHeader.AddCell(cellEmptyField);

                var cellInvoiceField = new PdfPTable(2);
                cellInvoiceField.SetWidths(new[] { 0.4f, 0.6f });
                cellInvoiceField.DefaultCell.Border = Rectangle.NO_BORDER;

                var cellInvoiceHeaderField = new PdfPCell(new Phrase("Invoice #", titleFont));
                cellInvoiceHeaderField.Border = Rectangle.NO_BORDER;
                cellInvoiceField.AddCell(cellInvoiceHeaderField);

                cellInvoiceHeaderField = new PdfPCell(new Phrase(order.Id.ToString(), font));
                cellInvoiceHeaderField.Border = Rectangle.NO_BORDER;
                cellInvoiceHeaderField.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellInvoiceField.AddCell(cellInvoiceHeaderField);

                cellInvoiceHeaderField = new PdfPCell(new Phrase("Date", titleFont));
                cellInvoiceHeaderField.Border = Rectangle.NO_BORDER;
                cellInvoiceField.AddCell(cellInvoiceHeaderField);

                cellInvoiceHeaderField = new PdfPCell(new Phrase(order.OrderPlaced.Value.ToString("D"), font));
                cellInvoiceHeaderField.Border = Rectangle.NO_BORDER;
                cellInvoiceHeaderField.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellInvoiceField.AddCell(cellInvoiceHeaderField);

                cellInvoiceHeader.AddCell(cellInvoiceField);

                cellLogoInvoice.AddCell(cellInvoiceHeader);

                headerTable.AddCell(cellLogoInvoice);

                doc.Add(headerTable);

                doc.Add(new Paragraph(" "));

                #endregion

                #region Addresses

                var addressTable = new PdfPTable(2);
                addressTable.DefaultCell.Border = Rectangle.NO_BORDER;
                addressTable.WidthPercentage = 100f;
                addressTable.SetWidths(new[] { 50, 50 });

                //billing info
                var billingAddress = new PdfPTable(1);
                billingAddress.DefaultCell.Border = Rectangle.NO_BORDER;

                billingAddress.AddCell(new Paragraph("Bill to:", titleFont));

                if (!string.IsNullOrEmpty(order.BillTo)) billingAddress.AddCell(new Paragraph(order.BillTo, font));
                if (!string.IsNullOrEmpty(order.AddressLine1)) billingAddress.AddCell(new Paragraph(order.AddressLine1, font));
                if (!string.IsNullOrEmpty(order.AddressLine2)) billingAddress.AddCell(new Paragraph(order.AddressLine2, font));
                if (!string.IsNullOrEmpty(order.City)) billingAddress.AddCell(new Paragraph(order.City, font));
                if (!string.IsNullOrEmpty(order.County)) billingAddress.AddCell(new Paragraph(order.County, font));
                if (!string.IsNullOrEmpty(order.PostCode)) billingAddress.AddCell(new Paragraph(order.PostCode, font));
                if (order.Country != null) billingAddress.AddCell(new Paragraph(order.Country.Name, font));

                addressTable.AddCell(billingAddress);

                //shipping info
                var shippingAddress = new PdfPTable(1);
                shippingAddress.DefaultCell.Border = Rectangle.NO_BORDER;

                shippingAddress.AddCell(new Paragraph("Ship to:", titleFont));

                if (!string.IsNullOrEmpty(order.ShipTo)) shippingAddress.AddCell(new Paragraph(order.ShipTo, font));
                if (!string.IsNullOrEmpty(order.ShippingAddressLine1)) shippingAddress.AddCell(new Paragraph(order.ShippingAddressLine1, font));
                if (!string.IsNullOrEmpty(order.ShippingAddressLine2)) shippingAddress.AddCell(new Paragraph(order.ShippingAddressLine2, font));
                if (!string.IsNullOrEmpty(order.ShippingCity)) shippingAddress.AddCell(new Paragraph(order.ShippingCity, font));
                if (!string.IsNullOrEmpty(order.ShippingCounty)) shippingAddress.AddCell(new Paragraph(order.ShippingCounty, font));
                if (!string.IsNullOrEmpty(order.ShippingPostCode)) shippingAddress.AddCell(new Paragraph(order.ShippingPostCode, font));
                if (order.ShippingCountry != null) shippingAddress.AddCell(new Paragraph(order.ShippingCountry.Name, font));

                addressTable.AddCell(shippingAddress);

                doc.Add(addressTable);
                doc.Add(new Paragraph(" "));

                #endregion

                #region Products

                //products
                var productsHeader = new PdfPTable(1);
                productsHeader.WidthPercentage = 100f;
                var cellProducts = new PdfPCell(new Phrase("Items", titleFont));
                cellProducts.Border = Rectangle.NO_BORDER;
                cellProducts.PaddingBottom = 5f;
                productsHeader.AddCell(cellProducts);
                doc.Add(productsHeader);

                var productsTable = new PdfPTable(4);
                productsTable.WidthPercentage = 100f;
                productsTable.SetWidths(new[] { 60, 15, 10, 15 });

                //product name
                var cellProductItem = new PdfPCell(new Phrase("Name", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                cellProductItem.PaddingTop = 5f;
                cellProductItem.PaddingBottom = 10f;
                cellProductItem.PaddingLeft = 5f;
                productsTable.AddCell(cellProductItem);

                //price
                cellProductItem = new PdfPCell(new Phrase("Price (excl. tax)", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellProductItem.PaddingTop = 5f;
                cellProductItem.PaddingBottom = 10f;
                cellProductItem.PaddingRight = 5f;
                productsTable.AddCell(cellProductItem);

                //qty
                cellProductItem = new PdfPCell(new Phrase("Quantity", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellProductItem.PaddingTop = 5f;
                cellProductItem.PaddingBottom = 10f;
                cellProductItem.PaddingRight = 5f;
                productsTable.AddCell(cellProductItem);

                //total
                cellProductItem = new PdfPCell(new Phrase("Line Total (excl. tax)", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellProductItem.PaddingTop = 5f;
                cellProductItem.PaddingBottom = 5f;
                cellProductItem.PaddingRight = 5f;
                productsTable.AddCell(cellProductItem);

                var orderItems = order.LineItemCollection.Where(x => ValidLineStatus.VALID_LINE_STATUSES.Contains(x.StatusCode)).ToList();
                foreach (var orderItem in orderItems)
                {
                    var pAttribTable = new PdfPTable(1);
                    pAttribTable.DefaultCell.Border = Rectangle.NO_BORDER;

                    //product name
                    string name = HttpUtility.HtmlDecode(orderItem.Name);
                    pAttribTable.AddCell(new Paragraph(name, font));
                    cellProductItem.AddElement(new Paragraph(name, font));

                    //attributes
                    if (!string.IsNullOrEmpty(orderItem.Option))
                    {
                        var attributesParagraph = new Paragraph(orderItem.Option, attributesFont);
                        pAttribTable.AddCell(attributesParagraph);
                    }
                    productsTable.AddCell(pAttribTable);

                    //price
                    string unitPrice = orderItem.CurrencyCode + " " + RoundPrice(orderItem.PriceExclTax * orderItem.ExchangeRate);

                    cellProductItem = new PdfPCell(new Phrase(unitPrice, font));
                    cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellProductItem.PaddingTop = 5f;
                    cellProductItem.PaddingBottom = 10f;
                    cellProductItem.PaddingRight = 5f;
                    productsTable.AddCell(cellProductItem);

                    //qty
                    cellProductItem = new PdfPCell(new Phrase(orderItem.Quantity.ToString(), font));
                    cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellProductItem.PaddingTop = 5f;
                    cellProductItem.PaddingBottom = 10f;
                    cellProductItem.PaddingRight = 5f;
                    productsTable.AddCell(cellProductItem);

                    //total                    
                    string subTotal = orderItem.CurrencyCode + " " + RoundPrice(orderItem.PriceExclTax * orderItem.Quantity * orderItem.ExchangeRate);

                    cellProductItem = new PdfPCell(new Phrase(subTotal, font));
                    cellProductItem.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellProductItem.PaddingTop = 5f;
                    cellProductItem.PaddingBottom = 10f;
                    cellProductItem.PaddingRight = 5f;

                    productsTable.AddCell(cellProductItem);
                }
                doc.Add(productsTable);

                #endregion

                #region Totals

                var totalsTable = new PdfPTable(2);
                totalsTable.SetWidths(new[] { 0.6f, 0.4f });
                totalsTable.DefaultCell.Border = Rectangle.NO_BORDER;
                totalsTable.WidthPercentage = 100f;

                cellEmptyField = new PdfPCell(new Phrase(""));
                cellEmptyField.Border = Rectangle.NO_BORDER;
                totalsTable.AddCell(cellEmptyField);

                var summaryTable = new PdfPTable(2);
                summaryTable.SetWidths(new[] { 0.6f, 0.4f });
                summaryTable.DefaultCell.Border = Rectangle.NO_BORDER;
                summaryTable.WidthPercentage = 100f;

                //order subtotal                
                var pSummaryField = new PdfPCell(new Phrase("Subtotal", font));
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                summaryTable.AddCell(pSummaryField);

                var subtotal = order.CurrencyCode + " " + RoundPrice(orderItems.Select(x => (x.PriceExclTax * x.Quantity * x.ExchangeRate)).Sum());
                pSummaryField = new PdfPCell(new Phrase(subtotal, font));
                pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                summaryTable.AddCell(pSummaryField);

                //discount (applied to order subtotal)
                if (order.DiscountAmount > decimal.Zero)
                {
                    var discount = order.CurrencyCode + " " + RoundPrice(order.DiscountAmount * order.ExchangeRate);
                    pSummaryField = new PdfPCell(new Phrase("Discount", font));
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);

                    pSummaryField = new PdfPCell(new Phrase(discount, font));
                    pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);
                }

                //used points
                if (order.AllocatedPoint > 0)
                {
                    var usedPointsValue = order.CurrencyCode + " " + RoundPrice(order.AllocatedPoint / 100M * order.ExchangeRate);
                    pSummaryField = new PdfPCell(new Phrase(string.Format("Used Points ({0} points)", order.AllocatedPoint), font));
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);

                    pSummaryField = new PdfPCell(new Phrase(usedPointsValue, font));
                    pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);
                }

                //shipping
                if (order.ShippingCost > decimal.Zero)
                {
                    var shipingCost = order.CurrencyCode + " " + RoundPrice(order.ShippingCost * order.ExchangeRate);
                    pSummaryField = new PdfPCell(new Phrase("Shipping", font));
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);

                    pSummaryField = new PdfPCell(new Phrase(shipingCost, font));
                    pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pSummaryField.Border = Rectangle.NO_BORDER;
                    pSummaryField.PaddingTop = 3f;
                    pSummaryField.PaddingBottom = 5f;
                    summaryTable.AddCell(pSummaryField);
                }

                //tax                
                pSummaryField = new PdfPCell(new Phrase("Tax", font));
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                summaryTable.AddCell(pSummaryField);

                var subTotalExclTax = orderItems.Select(x => (x.PriceExclTax * x.Quantity * x.ExchangeRate)).Sum();
                var subTotalInclTax = orderItems.Select(x => (x.PriceInclTax * x.Quantity * x.ExchangeRate)).Sum();
                var tax = order.CurrencyCode + " " + RoundPrice(subTotalInclTax - subTotalExclTax);
                pSummaryField = new PdfPCell(new Phrase(tax, font));
                pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                summaryTable.AddCell(pSummaryField);


                //order total
                var orderTotal = order.CurrencyCode + " " + RoundPrice(order.GrandTotal);
                pSummaryField = new PdfPCell(new Phrase("Total", font));
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                pSummaryField.BackgroundColor = BaseColor.LIGHT_GRAY;
                summaryTable.AddCell(pSummaryField);

                pSummaryField = new PdfPCell(new Phrase(orderTotal, font));
                pSummaryField.HorizontalAlignment = Element.ALIGN_RIGHT;
                pSummaryField.Border = Rectangle.NO_BORDER;
                pSummaryField.PaddingTop = 3f;
                pSummaryField.PaddingBottom = 5f;
                pSummaryField.BackgroundColor = BaseColor.LIGHT_GRAY;
                summaryTable.AddCell(pSummaryField);

                totalsTable.AddCell(summaryTable);

                doc.Add(totalsTable);

                #endregion

                ordNum++;
                if (ordNum < ordCount)
                {
                    doc.NewPage();
                }
            }

            doc.Close();
        }
        
        public void PrintOrderToInvoicePdf(Stream stream, Order order)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (order == null) throw new ArgumentNullException("order");

            var orders = new List<Order>();
            orders.Add(order);
            PrintOrderToInvoicePdf(stream, orders);
        }

        public void PrintBranchPendingLinePdf(Stream stream, IList<BranchPendingLineDownload> list)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (list == null) throw new ArgumentNullException("list");

            Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
            PdfWriter pdf = PdfWriter.GetInstance(pdfDoc, stream);

            pdfDoc.Open();

            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
            Font itemTitle = FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.BOLD);
            //Font addressFont = FontFactory.GetFont(FontFactory.HELVETICA, 9f);
            Font itemFont = FontFactory.GetFont(FontFactory.HELVETICA, 10f);
            Chunk titleText = new Chunk("Pending Note", titleFont);
            //Phrase lineBreak = new Phrase(Environment.NewLine);
            LineSeparator firstPageUnderLine = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -5f);
            //LineSeparator underLine = new LineSeparator(0.5f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, -10f);

            pdfDoc.Add(titleText);
            pdfDoc.Add(firstPageUnderLine);

            // Items
            pdfDoc.Add(GenerateItems(itemTitle, itemFont, list));

            pdfDoc.Close();
        }

        private PdfPTable GenerateItems(Font itemTitle, Font itemFont, IList<BranchPendingLineDownload> list)
        {
            PdfPTable itemsPending = new PdfPTable(new float[] { 35f, 10f, 15f, 10f, 10f });
            itemsPending.WidthPercentage = 100f;

            PdfPCell nameTitle = new PdfPCell(new Phrase(10f, "Name", itemTitle));
            nameTitle.HorizontalAlignment = LEFT;
            nameTitle.Border = 0;
            nameTitle.BorderWidthBottom = 0.5f;
            nameTitle.PaddingTop = 5f;
            nameTitle.PaddingBottom = 5f;

            PdfPCell sizeTitle = new PdfPCell(new Phrase(10f, "Option", itemTitle));
            sizeTitle.HorizontalAlignment = LEFT;
            sizeTitle.Border = 0;
            sizeTitle.BorderWidthBottom = 0.5f;
            sizeTitle.PaddingTop = 5f;
            sizeTitle.PaddingBottom = 5f;

            PdfPCell barcodeTitle = new PdfPCell(new Phrase(10f, "Barcode", itemTitle));
            barcodeTitle.HorizontalAlignment = LEFT;
            barcodeTitle.Border = 0;
            barcodeTitle.BorderWidthBottom = 0.5f;
            barcodeTitle.PaddingTop = 5f;
            barcodeTitle.PaddingBottom = 5f;

            PdfPCell quantityTitle = new PdfPCell(new Phrase(10f, "Quantity", itemTitle));
            quantityTitle.HorizontalAlignment = LEFT;
            quantityTitle.Border = 0;
            quantityTitle.BorderWidthBottom = 0.5f;
            quantityTitle.PaddingTop = 5f;
            quantityTitle.PaddingBottom = 5f;

            PdfPCell stockTitle = new PdfPCell(new Phrase(10f, "Stock", itemTitle));
            stockTitle.HorizontalAlignment = LEFT;
            stockTitle.Border = 0;
            stockTitle.BorderWidthBottom = 0.5f;
            stockTitle.PaddingTop = 5f;
            stockTitle.PaddingBottom = 5f;

            itemsPending.AddCell(nameTitle);
            itemsPending.AddCell(sizeTitle);
            itemsPending.AddCell(barcodeTitle);
            itemsPending.AddCell(quantityTitle);
            itemsPending.AddCell(stockTitle);

            // Items row
            PdfPCell cellName;
            PdfPCell cellOption;
            PdfPCell cellBarcode;
            PdfPCell cellQuantity;
            PdfPCell cellStock;

            for (int i = 0; i < list.Count; i++)
            {
                cellName = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(list[i].Name) + (list[i].Price > 0 ? string.Format(" - £{0:0.00}", list[i].Price) : ""), itemFont));
                cellName.HorizontalAlignment = LEFT;
                cellName.Border = 0;
                cellName.BorderWidthBottom = 0.5f;

                cellOption = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(list[i].Option), itemFont));
                cellOption.HorizontalAlignment = LEFT;
                cellOption.Border = 0;
                cellOption.BorderWidthBottom = 0.5f;

                string barcode = string.Empty;

                if (list[i].Barcode != string.Empty)
                    barcode = list[i].Barcode;

                cellBarcode = new PdfPCell(new Phrase(10f, barcode, itemFont));
                cellBarcode.HorizontalAlignment = LEFT;
                cellBarcode.Border = 0;
                cellBarcode.BorderWidthBottom = 0.5f;

                cellQuantity = new PdfPCell(new Phrase(10f, list[i].Quantity.ToString(), itemFont));
                cellQuantity.HorizontalAlignment = LEFT;
                cellQuantity.Border = 0;
                cellQuantity.BorderWidthBottom = 0.5f;

                var stock = list[i].Stock;                
                cellStock = new PdfPCell(new Phrase(10f, stock.ToString(), itemFont));
                cellStock.HorizontalAlignment = LEFT;
                cellStock.Border = 0;
                cellStock.BorderWidthBottom = 0.5f;

                itemsPending.AddCell(cellName);
                itemsPending.AddCell(cellOption);
                itemsPending.AddCell(cellBarcode);
                itemsPending.AddCell(cellQuantity);
                itemsPending.AddCell(cellStock);
            }

            return itemsPending;
        }

        private PdfPTable GenerateItems(Font itemTitle, Font itemFont, IList<InventoryPendingLine> list)
        {
            PdfPTable itemsPending = new PdfPTable(new float[] { 10f, 40f, 10f, 10f, 10f, 10f, 10f });
            itemsPending.WidthPercentage = 100f;

            PdfPCell productIdTitle = new PdfPCell(new Phrase(10f, "Product ID", itemTitle));
            productIdTitle.HorizontalAlignment = LEFT;
            productIdTitle.Border = 0;
            productIdTitle.BorderWidthBottom = 0.5f;
            productIdTitle.PaddingTop = 5f;
            productIdTitle.PaddingBottom = 5f;

            PdfPCell nameTitle = new PdfPCell(new Phrase(10f, "Name", itemTitle));
            nameTitle.HorizontalAlignment = LEFT;
            nameTitle.Border = 0;
            nameTitle.BorderWidthBottom = 0.5f;
            nameTitle.PaddingTop = 5f;
            nameTitle.PaddingBottom = 5f;

            PdfPCell sizeTitle = new PdfPCell(new Phrase(10f, "Option", itemTitle));
            sizeTitle.HorizontalAlignment = LEFT;
            sizeTitle.Border = 0;
            sizeTitle.BorderWidthBottom = 0.5f;
            sizeTitle.PaddingTop = 5f;
            sizeTitle.PaddingBottom = 5f;
            
            PdfPCell quantityTitle = new PdfPCell(new Phrase(10f, "Quantity", itemTitle));
            quantityTitle.HorizontalAlignment = LEFT;
            quantityTitle.Border = 0;
            quantityTitle.BorderWidthBottom = 0.5f;
            quantityTitle.PaddingTop = 5f;
            quantityTitle.PaddingBottom = 5f;

            PdfPCell stockTitle = new PdfPCell(new Phrase(10f, "Stock", itemTitle));
            stockTitle.HorizontalAlignment = LEFT;
            stockTitle.Border = 0;
            stockTitle.BorderWidthBottom = 0.5f;
            stockTitle.PaddingTop = 5f;
            stockTitle.PaddingBottom = 5f;

            PdfPCell orderPlacedTitle = new PdfPCell(new Phrase(10f, "Order Placed", itemTitle));
            orderPlacedTitle.HorizontalAlignment = LEFT;
            orderPlacedTitle.Border = 0;
            orderPlacedTitle.BorderWidthBottom = 0.5f;
            orderPlacedTitle.PaddingTop = 5f;
            orderPlacedTitle.PaddingBottom = 5f;

            PdfPCell orderCountTitle = new PdfPCell(new Phrase(10f, "Order Count", itemTitle));
            orderCountTitle.HorizontalAlignment = LEFT;
            orderCountTitle.Border = 0;
            orderCountTitle.BorderWidthBottom = 0.5f;
            orderCountTitle.PaddingTop = 5f;
            orderCountTitle.PaddingBottom = 5f;

            itemsPending.AddCell(productIdTitle);
            itemsPending.AddCell(nameTitle);
            itemsPending.AddCell(sizeTitle);
            itemsPending.AddCell(quantityTitle);
            itemsPending.AddCell(stockTitle);
            itemsPending.AddCell(orderPlacedTitle);
            itemsPending.AddCell(orderCountTitle);

            // Items row
            PdfPCell cellProductId;
            PdfPCell cellName;
            PdfPCell cellOption;            
            PdfPCell cellQuantity;
            PdfPCell cellStock;
            PdfPCell cellOrderPlaced;
            PdfPCell cellOrderCount;

            for (int i = 0; i < list.Count; i++)
            {
                cellProductId = new PdfPCell(new Phrase(10f, list[i].ProductId.ToString(), itemFont));
                cellProductId.HorizontalAlignment = LEFT;
                cellProductId.Border = 0;
                cellProductId.BorderWidthBottom = 0.5f;

                cellName = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(list[i].Name), itemFont));
                cellName.HorizontalAlignment = LEFT;
                cellName.Border = 0;
                cellName.BorderWidthBottom = 0.5f;

                var option = "";
                if (string.IsNullOrEmpty(list[i].Option) == false)
                {
                    option = list[i].Option;
                }
                cellOption = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(option), itemFont));
                cellOption.HorizontalAlignment = LEFT;
                cellOption.Border = 0;
                cellOption.BorderWidthBottom = 0.5f;

                cellQuantity = new PdfPCell(new Phrase(10f, list[i].Quantity.ToString(), itemFont));
                cellQuantity.HorizontalAlignment = LEFT;
                cellQuantity.Border = 0;
                cellQuantity.BorderWidthBottom = 0.5f;

                cellStock = new PdfPCell(new Phrase(10f, list[i].Stock.ToString(), itemFont));
                cellStock.HorizontalAlignment = LEFT;
                cellStock.Border = 0;
                cellStock.BorderWidthBottom = 0.5f;

                cellOrderPlaced = new PdfPCell(new Phrase(10f, list[i].OrderPlaced.ToString("dd/MM/yyyy"), itemFont));
                cellOrderPlaced.HorizontalAlignment = LEFT;
                cellOrderPlaced.Border = 0;
                cellOrderPlaced.BorderWidthBottom = 0.5f;

                cellOrderCount = new PdfPCell(new Phrase(10f, list[i].OrderCount.ToString(), itemFont));
                cellOrderCount.HorizontalAlignment = LEFT;
                cellOrderCount.Border = 0;
                cellOrderCount.BorderWidthBottom = 0.5f;

                itemsPending.AddCell(cellProductId);
                itemsPending.AddCell(cellName);
                itemsPending.AddCell(cellOption);                
                itemsPending.AddCell(cellQuantity);
                itemsPending.AddCell(cellStock);
                itemsPending.AddCell(cellOrderPlaced);
                itemsPending.AddCell(cellOrderCount);
            }

            return itemsPending;
        }

        private PdfPTable GenerateItems(Font itemTitle, Font itemFont, IList<PickingLineItem> list)
        {
            PdfPTable itemsPending = new PdfPTable(new float[] { 50f, 10f, 20f, 5f, 10f, 5f });
            itemsPending.WidthPercentage = 100f;
            
            PdfPCell nameTitle = new PdfPCell(new Phrase(10f, "Name", itemTitle));
            nameTitle.HorizontalAlignment = LEFT;
            nameTitle.Border = 0;
            nameTitle.BorderWidthBottom = 0.5f;
            nameTitle.PaddingTop = 5f;
            nameTitle.PaddingBottom = 5f;

            PdfPCell optionTitle = new PdfPCell(new Phrase(10f, "Option", itemTitle));
            optionTitle.HorizontalAlignment = LEFT;
            optionTitle.Border = 0;
            optionTitle.BorderWidthBottom = 0.5f;
            optionTitle.PaddingTop = 5f;
            optionTitle.PaddingBottom = 5f;

            PdfPCell barcodeTitle = new PdfPCell(new Phrase(10f, "Barcode", itemTitle));
            barcodeTitle.HorizontalAlignment = LEFT;
            barcodeTitle.Border = 0;
            barcodeTitle.BorderWidthBottom = 0.5f;
            barcodeTitle.PaddingTop = 5f;
            barcodeTitle.PaddingBottom = 5f;

            PdfPCell quantityTitle = new PdfPCell(new Phrase(10f, "Qty", itemTitle));
            quantityTitle.HorizontalAlignment = LEFT;
            quantityTitle.Border = 0;
            quantityTitle.BorderWidthBottom = 0.5f;
            quantityTitle.PaddingTop = 5f;
            quantityTitle.PaddingBottom = 5f;

            PdfPCell branchTitle = new PdfPCell(new Phrase(10f, "Branch", itemTitle));
            branchTitle.HorizontalAlignment = LEFT;
            branchTitle.Border = 0;
            branchTitle.BorderWidthBottom = 0.5f;
            branchTitle.PaddingTop = 5f;
            branchTitle.PaddingBottom = 5f;

            PdfPCell wstockTitle = new PdfPCell(new Phrase(10f, "W/Stock", itemTitle));
            wstockTitle.HorizontalAlignment = LEFT;
            wstockTitle.Border = 0;
            wstockTitle.BorderWidthBottom = 0.5f;
            wstockTitle.PaddingTop = 5f;
            wstockTitle.PaddingBottom = 5f;

            itemsPending.AddCell(nameTitle);
            itemsPending.AddCell(optionTitle);
            itemsPending.AddCell(barcodeTitle);
            itemsPending.AddCell(quantityTitle);
            itemsPending.AddCell(branchTitle);
            itemsPending.AddCell(wstockTitle);
            
            // Items row
            PdfPCell cellName;
            PdfPCell cellOption;
            PdfPCell cellBarcode;
            PdfPCell cellQuantity;
            PdfPCell cellBranch;
            PdfPCell cellWStock;

            for (int i = 0; i < list.Count; i++)
            {
                cellName = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(list[i].Name), itemFont));
                cellName.HorizontalAlignment = LEFT;
                cellName.Border = 0;
                cellName.BorderWidthBottom = 0.5f;

                var option = "";
                if (string.IsNullOrEmpty(list[i].Option) == false)
                {
                    option = list[i].Option;
                }
                cellOption = new PdfPCell(new Phrase(10f, HttpUtility.HtmlDecode(option), itemFont));
                cellOption.HorizontalAlignment = LEFT;
                cellOption.Border = 0;
                cellOption.BorderWidthBottom = 0.5f;

                cellBarcode = new PdfPCell(new Phrase(10f, list[i].Barcode, itemFont));
                cellBarcode.HorizontalAlignment = LEFT;
                cellBarcode.Border = 0;
                cellBarcode.BorderWidthBottom = 0.5f;

                cellQuantity = new PdfPCell(new Phrase(10f, list[i].PendingQuantity.ToString(), itemFont));
                cellQuantity.HorizontalAlignment = LEFT;
                cellQuantity.Border = 0;
                cellQuantity.BorderWidthBottom = 0.5f;

                cellBranch = new PdfPCell(new Phrase(10f, list[i].SelectedBranch.ToString(), itemFont));
                cellBranch.HorizontalAlignment = LEFT;
                cellBranch.Border = 0;
                cellBranch.BorderWidthBottom = 0.5f;

                var stock = 0;
                if (list[i].WarehouseStock != null)
                    stock = list[i].WarehouseStock.Stock;

                cellWStock = new PdfPCell(new Phrase(10f, stock.ToString(), itemFont));
                cellWStock.HorizontalAlignment = LEFT;
                cellWStock.Border = 0;
                cellWStock.BorderWidthBottom = 0.5f;

                itemsPending.AddCell(cellName);
                itemsPending.AddCell(cellOption);
                itemsPending.AddCell(cellBarcode);
                itemsPending.AddCell(cellQuantity);
                itemsPending.AddCell(cellBranch);
                itemsPending.AddCell(cellWStock);
            }

            return itemsPending;
        }

        #region Utilities

        protected virtual Font GetFont()
        {
            //supports unicode characters
            //uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont

            //string fontPath = Path.Combine(_webHelper.MapPath("~/App_Data/Pdf/"), _pdfSettings.FontFileName);
            //var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //var font = new Font(baseFont, 10, Font.NORMAL);
            //return font;

            return FontFactory.GetFont(FontFactory.HELVETICA, 10f, Font.NORMAL);
        }

        /// <summary>
        /// Get font direction
        /// </summary>
        /// <param name="lang">Language</param>
        /// <returns>Font direction</returns>
        //protected virtual int GetDirection(Language lang)
        //{
        //    return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
        //}

        /// <summary>
        /// Get element alignment
        /// </summary>
        /// <param name="lang">Language</param>
        /// <param name="isOpposite">Is opposite?</param>
        /// <returns>Element alignment</returns>
        //protected virtual int GetAlignment(Language lang, bool isOpposite = false)
        //{
        //    //if we need the element to be opposite, like logo etc`.
        //    if (!isOpposite)
        //        return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;

        //    return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
        //}

        private decimal RoundPrice(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        #endregion
    }
}
