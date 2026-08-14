using Helth.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace Helth.Services;

public class PdfService : IPdfService
{
    private const string FontFamily = "Noto Sans Arabic";

    // Sampled directly from reference-design/health-certificate-sample.png
    private const string HeaderGreen = "#7BBC47";
    private const string NameLightBlue = "#2D9CDB";
    private const string DividerGray = "#E3E7E8";
    private const string LabelGray = "#6B7280";
    private const string FieldBoxBackground = "#F7F8F9";
    private const string FieldBoxBorder = "#DDE1E3";

    private const int PhotoBoxSize = 104;
    private const int PhotoQrGap = 8;
    private const int PhotoSourceSize = 400;

    private readonly IQrCodeService _qrCodeService;
    private static bool _fontsRegistered;
    private static readonly object FontLock = new();

    public PdfService(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    private static void EnsureFontsRegistered(string webRootPath)
    {
        if (_fontsRegistered)
        {
            return;
        }

        lock (FontLock)
        {
            if (_fontsRegistered)
            {
                return;
            }

            var regularPath = Path.Combine(webRootPath, "fonts", "NotoSansArabic-Regular.ttf");
            var boldPath = Path.Combine(webRootPath, "fonts", "NotoSansArabic-Bold.ttf");

            if (File.Exists(regularPath))
            {
                using var regularStream = File.OpenRead(regularPath);
                FontManager.RegisterFont(regularStream);
            }

            if (File.Exists(boldPath))
            {
                using var boldStream = File.OpenRead(boldPath);
                FontManager.RegisterFont(boldStream);
            }

            _fontsRegistered = true;
        }
    }

    // Center-crops the source image to a square so it can fill its PDF container edge-to-edge
    // (QuestPDF's Image element has no built-in "cover" mode - only "contain"-style fitting).
    // Also flattens any transparency onto white: a transparent source (common for uploaded PNG
    // headshots) would otherwise show the white page through the "empty" corners, which looks
    // identical to the image not filling its frame.
    private static byte[] CropToSquareCover(byte[] source, int size)
    {
        using var image = ImageSharpImage.Load<Rgba32>(source);
        image.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(size, size),
                Mode = ResizeMode.Crop
            })
            .BackgroundColor(SixLabors.ImageSharp.Color.White));

        using var ms = new MemoryStream();
        image.SaveAsJpeg(ms);
        return ms.ToArray();
    }

    public byte[] GenerateEmployeeCertificate(Employee employee, string publicUrl, string webRootPath)
    {
        EnsureFontsRegistered(webRootPath);

        var qrBytes = _qrCodeService.GeneratePngBytes(publicUrl);

        byte[]? photoBytes = null;
        if (!string.IsNullOrEmpty(employee.PhotoPath))
        {
            var photoFullPath = Path.Combine(webRootPath, employee.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(photoFullPath))
            {
                photoBytes = CropToSquareCover(File.ReadAllBytes(photoFullPath), PhotoSourceSize);
            }
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontFamily(FontFamily).FontSize(11).DirectionFromRightToLeft());

                // Card container: rounded corners + border + drop shadow, like a physical ID card.
                page.Content().Container()
                    .Background(Colors.White)
                    .Border(1f).BorderColor(DividerGray)
                    .CornerRadius(14)
                    .Shadow(new BoxShadowStyle
                    {
                        Color = "#00000026",
                        Blur = 10,
                        Spread = 0,
                        OffsetX = 0,
                        OffsetY = 4
                    })
                    .Column(column =>
                    {
                        // Header: green title box hugging the LEFT edge (rounded to match the card's
                        // top-left corner), logos anchored to the RIGHT edge, same row.
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(190).Background(HeaderGreen).CornerRadiusTopLeft(13)
                                .Padding(14).AlignMiddle()
                                .Text("شهادة صحية").FontColor(Colors.White).FontSize(22).Bold();

                            row.RelativeItem().Background(Colors.White).CornerRadiusTopRight(13)
                                .Padding(10).AlignMiddle().AlignRight()
                                .Row(logoRow =>
                                {
                                    logoRow.AutoItem().Element(e => RenderLogoPlaceholder(e, "الشعار الوطني"));
                                    logoRow.AutoItem().Width(8);
                                    logoRow.AutoItem().Element(e => RenderLogoPlaceholder(e, "بلدي"));
                                    logoRow.AutoItem().Width(8);
                                    logoRow.AutoItem().Element(e => RenderLogoPlaceholder(e, "الوزارة"));
                                });
                        });

                        // Employee name - full width, above the photo/fields block (not part of
                        // either column), so the fields below can align exactly with the photo+QR.
                        column.Item().PaddingHorizontal(24).PaddingTop(20).AlignRight()
                            .Text(employee.FullName).FontColor(NameLightBlue).Bold().FontSize(19);

                        column.Item().PaddingHorizontal(24).PaddingTop(8)
                            .LineHorizontal(0.75f).LineColor(DividerGray);

                        // Compact ID-card body: photo+QR stacked on the left, 3 field rows on the
                        // right, both columns spanning the exact same total height (PhotoQrTotalHeight)
                        // so the fields block starts/ends flush with the photo/QR column.
                        column.Item().PaddingHorizontal(24).PaddingTop(16).Row(row =>
                        {
                            row.ConstantItem(130).Column(photoCol =>
                            {
                                photoCol.Item().Border(1.5f).BorderColor(HeaderGreen)
                                    .Height(PhotoBoxSize).Width(PhotoBoxSize)
                                    .Element(e =>
                                    {
                                        if (photoBytes != null)
                                        {
                                            e.Image(photoBytes).FitUnproportionally();
                                        }
                                        else
                                        {
                                            e.Background(FieldBoxBackground).AlignCenter().AlignMiddle()
                                                .Text("لا توجد صورة").FontSize(9).FontColor(LabelGray);
                                        }
                                    });

                                photoCol.Item().PaddingTop(PhotoQrGap).Border(1f).BorderColor(HeaderGreen)
                                    .Height(PhotoBoxSize).Width(PhotoBoxSize).Image(qrBytes).FitUnproportionally();
                            });

                            row.RelativeItem().PaddingLeft(20).Column(infoCol =>
                            {
                                // 3 field rows evenly distributed across the same total height as
                                // the photo+gap+QR column, so top/bottom edges line up exactly.
                                const int fieldRowHeight = (PhotoBoxSize * 2 + PhotoQrGap) / 3;

                                infoCol.Item().Height(fieldRowHeight).AlignMiddle().Row(r =>
                                {
                                    r.RelativeItem().Element(e => RenderField(e, "الجنسية", employee.Nationality));
                                    r.RelativeItem().Element(e => RenderField(e, "رقم الهوية", employee.NationalId));
                                });

                                infoCol.Item().Height(fieldRowHeight).AlignMiddle().Row(r =>
                                {
                                    r.RelativeItem().Element(e => RenderField(e, "المهنة", employee.Profession));
                                    r.RelativeItem().Element(e => RenderField(e, "رقم الشهادة الصحية", employee.HealthCertificateNumber));
                                });

                                infoCol.Item().Height(fieldRowHeight).AlignMiddle().Row(r =>
                                {
                                    r.RelativeItem().Element(e => RenderField(e, "تاريخ انتهاء البرنامج التثقيفي", employee.TrainingProgramExpiryDate.ToString("yyyy/MM/dd")));
                                    r.RelativeItem().Element(e => RenderField(e, "تاريخ إصدار الشهادة الصحية", employee.IssueDateGregorian.ToString("yyyy/MM/dd")));
                                });
                            });
                        });

                        // Footer sits directly below the last content row (no page-bottom pinning).
                        column.Item().PaddingTop(20).BorderTop(0.75f).BorderColor(DividerGray)
                            .Padding(10).Row(row =>
                            {
                                row.RelativeItem().Element(e => RenderFooterItem(e, "199040"));
                                row.RelativeItem().Element(e => RenderFooterItem(e, "Balady_cs"));
                                row.RelativeItem().Element(e => RenderFooterItem(e, "saudimomra"));
                                row.RelativeItem().Element(e => RenderFooterItem(e, "info@balady.gov.sa"));
                            });

                        // Instructions section - placeholder text, to be replaced with real content later.
                        // ExtendVertical() stretches this section (and the card that contains it) down
                        // to the bottom of the page, leaving no leftover white gap.
                        column.Item().ExtendVertical().Background(HeaderGreen)
                            .CornerRadiusBottomLeft(13).CornerRadiusBottomRight(13)
                            .Padding(16).Column(instructionsCol =>
                            {
                                instructionsCol.Item().AlignRight()
                                    .Text("تعليمات وإرشادات").FontColor(Colors.White).Bold().FontSize(14);

                                instructionsCol.Item().PaddingTop(6).AlignRight()
                                    .Text("هذا نص تعليمات مؤقت (Placeholder) - سيتم استبداله بالمحتوى الفعلي لاحقاً.")
                                    .FontColor(Colors.White).FontSize(10);

                                instructionsCol.Item().PaddingTop(3).AlignRight()
                                    .Text("سطر إضافي للتعليمات سيتم تحديثه لاحقاً بالنص النهائي.")
                                    .FontColor(Colors.White).FontSize(10);
                            });
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static void RenderLogoPlaceholder(IContainer container, string label)
    {
        container.Height(36).Width(50).Border(0.75f).BorderColor(HeaderGreen)
            .AlignCenter().AlignMiddle().Text(label).FontSize(6).FontColor(HeaderGreen).Bold();
    }

    // Renders a label above a bordered, read-only-looking value box (mirrors the web app's
    // disabled form-field styling instead of plain label/value text).
    private static void RenderField(IContainer container, string label, string value)
    {
        container.PaddingRight(12).Column(col =>
        {
            col.Item().Text(label).Bold().FontSize(9.5f).FontColor(LabelGray);
            col.Item().PaddingTop(4).Border(1f).BorderColor(FieldBoxBorder).Background(FieldBoxBackground)
                .CornerRadius(4).Padding(6)
                .Text(string.IsNullOrEmpty(value) ? "-" : value).FontSize(11).FontColor(Colors.Black);
        });
    }

    private static void RenderFooterItem(IContainer container, string text)
    {
        container.AlignCenter().Row(r =>
        {
            r.AutoItem().Height(8).Width(8).Background(HeaderGreen);
            r.AutoItem().Width(4);
            r.AutoItem().Text(text).FontSize(9).FontColor(LabelGray);
        });
    }
}
