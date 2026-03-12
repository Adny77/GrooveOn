import 'dart:io';

import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:grooveon_desktop/models/income_by_month_response.dart';
import 'package:grooveon_desktop/providers/report_provider.dart';
import 'package:grooveon_desktop/screens/income_screen.dart';
import 'package:intl/intl.dart';
import 'package:open_filex/open_filex.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:provider/provider.dart';

class IncomeDetailedContent extends StatefulWidget {
  final int selectedYear;

  const IncomeDetailedContent({
    super.key,
    required this.selectedYear,
  });

  @override
  State<IncomeDetailedContent> createState() => _IncomeDetailedContentState();
}

class _IncomeDetailedContentState extends State<IncomeDetailedContent> {
  late Future<List<IncomeByMonthResponse>> _futureIncome;

  @override
  void initState() {
    super.initState();
    _futureIncome = _loadIncome();
  }

  @override
  void didUpdateWidget(covariant IncomeDetailedContent oldWidget) {
    super.didUpdateWidget(oldWidget);

    if (oldWidget.selectedYear != widget.selectedYear) {
      setState(() {
        _futureIncome = _loadIncome();
      });
    }
  }

  Future<List<IncomeByMonthResponse>> _loadIncome() {
    final provider = context.read<ReportProvider>();
    return provider.getIncomeByMonth(year: widget.selectedYear);
  }

  List<_IncomeRowData> _buildRows(
    int year,
    List<IncomeByMonthResponse> data,
  ) {
    const monthNames = [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ];

    final now = DateTime.now();
    final lastCompletedMonth = year < now.year
        ? 12
        : year == now.year
            ? now.month - 1
            : 0;

    final incomeMap = <int, double>{};
    for (final item in data) {
      incomeMap[item.month] = item.totalIncome;
    }

    return List.generate(12, (index) {
      final monthNumber = index + 1;
      final monthName = monthNames[index];

      if (monthNumber <= lastCompletedMonth) {
        return _IncomeRowData(
          month: monthName,
          premiumIncome: incomeMap[monthNumber] ?? 0,
          isAvailable: true,
        );
      }

      return _IncomeRowData(
        month: monthName,
        premiumIncome: null,
        isAvailable: false,
      );
    });
  }

  double _calculateAverageMonthlyIncrease(List<_IncomeRowData> rows) {
    final availableValues = rows
        .where((e) => e.isAvailable && e.premiumIncome != null)
        .map((e) => e.premiumIncome!)
        .toList();

    if (availableValues.length < 2) return 0;

    double totalPercentage = 0;
    int count = 0;

    for (int i = 1; i < availableValues.length; i++) {
      final previous = availableValues[i - 1];
      final current = availableValues[i];

      if (previous == 0) continue;

      final increase = ((current - previous) / previous) * 100;
      totalPercentage += increase;
      count++;
    }

    if (count == 0) return 0;
    return totalPercentage / count;
  }

  Future<List<int>> _buildIncomePdfBytes({
    required int year,
    required List<_IncomeRowData> rows,
    required double profitIncreasePercentage,
  }) async {
    final doc = pw.Document();

    final now = DateTime.now();
    final dateFmt = DateFormat('dd.MM.yyyy HH:mm');

    final darkPurple = PdfColor.fromHex("#4A148C");
    final midPurple = PdfColor.fromHex("#9C27B0");
    final lightPurpleBg = PdfColor.fromHex("#F3E5F5");
    final borderColor = PdfColor.fromHex("#D9D9DE");
    final textColor = PdfColor.fromHex("#222222");
    final subTextColor = PdfColor.fromHex("#6F6F78");
    final tableHeaderBg = PdfColor.fromHex("#4A148C");
    final evenRowBg = PdfColor.fromHex("#FAFAFC");

    final availableRows =
        rows.where((e) => e.isAvailable && e.premiumIncome != null).toList();

    final totalPremiumIncome = availableRows.fold<double>(
      0,
      (sum, item) => sum + (item.premiumIncome ?? 0),
    );

    final averagePerCompletedMonth =
        availableRows.isEmpty ? 0 : totalPremiumIncome / availableRows.length;

    _IncomeRowData? bestMonth;
    if (availableRows.isNotEmpty) {
      bestMonth = availableRows.reduce((a, b) {
        final aVal = a.premiumIncome ?? 0;
        final bVal = b.premiumIncome ?? 0;
        return aVal >= bVal ? a : b;
      });
    }

    doc.addPage(
      pw.MultiPage(
        pageFormat: PdfPageFormat.a4,
        margin: const pw.EdgeInsets.fromLTRB(28, 28, 28, 28),
        build: (context) => [
          pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Text(
                    "GrooveOn Income Report",
                    style: pw.TextStyle(
                      fontSize: 22,
                      fontWeight: pw.FontWeight.bold,
                      color: textColor,
                    ),
                  ),
                  pw.SizedBox(height: 4),
                  pw.Text(
                    "Generated: ${dateFmt.format(now)}",
                    style: pw.TextStyle(
                      fontSize: 10,
                      color: subTextColor,
                    ),
                  ),
                ],
              ),
              pw.Container(
                padding: const pw.EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 7,
                ),
                decoration: pw.BoxDecoration(
                  color: lightPurpleBg,
                  borderRadius: pw.BorderRadius.circular(8),
                  border: pw.Border.all(color: midPurple, width: 1),
                ),
                child: pw.Text(
                  "Year: $year",
                  style: pw.TextStyle(
                    fontSize: 11,
                    fontWeight: pw.FontWeight.bold,
                    color: darkPurple,
                  ),
                ),
              ),
            ],
          ),
          pw.SizedBox(height: 18),
          pw.Row(
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              pw.Expanded(
                child: _buildPdfKpiCard(
                  title: "Total premium income",
                  value: "${totalPremiumIncome.toStringAsFixed(2)} KM",
                  darkPurple: darkPurple,
                  borderColor: borderColor,
                  subTextColor: subTextColor,
                ),
              ),
              pw.SizedBox(width: 10),
              pw.Expanded(
                child: _buildPdfKpiCard(
                  title: "Best month",
                  value: bestMonth == null
                      ? "No data"
                      : "${bestMonth.month} - ${bestMonth.premiumIncome!.toStringAsFixed(2)} KM",
                  darkPurple: darkPurple,
                  borderColor: borderColor,
                  subTextColor: subTextColor,
                ),
              ),
              pw.SizedBox(width: 10),
              pw.Expanded(
                child: _buildPdfKpiCard(
                  title: "Average per completed month",
                  value: "${averagePerCompletedMonth.toStringAsFixed(2)} KM",
                  darkPurple: darkPurple,
                  borderColor: borderColor,
                  subTextColor: subTextColor,
                ),
              ),
            ],
          ),
          pw.SizedBox(height: 22),
          pw.Text(
            "Income details",
            style: pw.TextStyle(
              fontSize: 14,
              fontWeight: pw.FontWeight.bold,
              color: textColor,
            ),
          ),
          pw.SizedBox(height: 8),
          pw.Table(
            border: pw.TableBorder.all(color: borderColor, width: 0.8),
            columnWidths: {
              0: const pw.FlexColumnWidth(3),
              1: const pw.FlexColumnWidth(5),
            },
            children: [
              pw.TableRow(
                decoration: pw.BoxDecoration(color: tableHeaderBg),
                children: [
                  _pdfHeaderCell("Month"),
                  _pdfHeaderCell("Premium income"),
                ],
              ),
              ...List.generate(rows.length, (index) {
                final row = rows[index];
                final isEven = index % 2 == 0;

                return pw.TableRow(
                  decoration: pw.BoxDecoration(
                    color: isEven ? evenRowBg : PdfColors.white,
                  ),
                  children: [
                    _pdfBodyCell(row.month, textColor),
                    _pdfBodyCell(
                      row.isAvailable && row.premiumIncome != null
                          ? "${row.premiumIncome!.toStringAsFixed(2)} KM"
                          : "x",
                      textColor,
                    ),
                  ],
                );
              }),
            ],
          ),
          pw.SizedBox(height: 18),
          pw.Container(
            width: double.infinity,
            padding: const pw.EdgeInsets.all(14),
            decoration: pw.BoxDecoration(
              color: lightPurpleBg,
              borderRadius: pw.BorderRadius.circular(10),
              border: pw.Border.all(color: midPurple),
            ),
            child: pw.Column(
              crossAxisAlignment: pw.CrossAxisAlignment.start,
              children: [
                pw.Text(
                  "Average profit increase per month",
                  style: pw.TextStyle(
                    fontSize: 12,
                    fontWeight: pw.FontWeight.bold,
                    color: darkPurple,
                  ),
                ),
                pw.SizedBox(height: 8),
                pw.Text(
                  "${profitIncreasePercentage.toStringAsFixed(2)}%",
                  style: pw.TextStyle(
                    fontSize: 20,
                    fontWeight: pw.FontWeight.bold,
                    color: textColor,
                  ),
                ),
              ],
            ),
          ),
        ],
        footer: (context) => pw.Row(
          mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
          children: [
            pw.Text(
              "GrooveOn internal report",
              style: pw.TextStyle(
                fontSize: 10,
                color: subTextColor,
              ),
            ),
            pw.Text(
              "Page ${context.pageNumber} / ${context.pagesCount}",
              style: pw.TextStyle(
                fontSize: 10,
                color: subTextColor,
              ),
            ),
          ],
        ),
      ),
    );

    return doc.save();
  }

  pw.Widget _buildPdfKpiCard({
    required String title,
    required String value,
    required PdfColor darkPurple,
    required PdfColor borderColor,
    required PdfColor subTextColor,
  }) {
    return pw.Container(
      padding: const pw.EdgeInsets.all(12),
      decoration: pw.BoxDecoration(
        color: PdfColors.white,
        borderRadius: pw.BorderRadius.circular(10),
        border: pw.Border.all(color: borderColor),
      ),
      child: pw.Column(
        crossAxisAlignment: pw.CrossAxisAlignment.start,
        children: [
          pw.Text(
            title,
            style: pw.TextStyle(
              fontSize: 10,
              color: subTextColor,
            ),
          ),
          pw.SizedBox(height: 8),
          pw.Text(
            value,
            style: pw.TextStyle(
              fontSize: 14,
              fontWeight: pw.FontWeight.bold,
              color: darkPurple,
            ),
          ),
        ],
      ),
    );
  }

  pw.Widget _pdfHeaderCell(String text) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(horizontal: 8, vertical: 8),
      child: pw.Text(
        text,
        style: pw.TextStyle(
          color: PdfColors.white,
          fontWeight: pw.FontWeight.bold,
          fontSize: 10,
        ),
      ),
    );
  }

  pw.Widget _pdfBodyCell(String text, PdfColor textColor) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(horizontal: 8, vertical: 7),
      child: pw.Text(
        text,
        style: pw.TextStyle(
          color: textColor,
          fontSize: 10,
        ),
      ),
    );
  }

  Future<void> _printIncomeReport({
    required int year,
    required List<_IncomeRowData> rows,
    required double profitIncreasePercentage,
  }) async {
    final path = await FilePicker.platform.saveFile(
      dialogTitle: 'Save income report PDF',
      fileName: 'GrooveOn_Income_Report_$year.pdf',
      type: FileType.custom,
      allowedExtensions: ['pdf'],
    );

    if (path == null) return;

    try {
      final bytes = await _buildIncomePdfBytes(
        year: year,
        rows: rows,
        profitIncreasePercentage: profitIncreasePercentage,
      );

      final file = File(path);
      await file.writeAsBytes(bytes, flush: true);
      await OpenFilex.open(file.path);
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Cannot create PDF: $e")),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<IncomeByMonthResponse>>(
      future: _futureIncome,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Container(
                  height: 610,
                  decoration: BoxDecoration(
                    color: Colors.white,
                    border: Border.all(color: IncomeScreen.borderColor),
                  ),
                  child: const Center(
                    child: CircularProgressIndicator(),
                  ),
                ),
              ),
              const SizedBox(width: 18),
              const _ProfitIncreaseCard(
                profitIncreasePercentage: 0,
              ),
            ],
          );
        }

        if (snapshot.hasError) {
          return Container(
            width: double.infinity,
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border.all(color: IncomeScreen.borderColor),
            ),
            child: Text(
              "Greska pri ucitavanju detalja prihoda: ${snapshot.error}",
              style: const TextStyle(
                color: Colors.red,
                fontSize: 14,
              ),
            ),
          );
        }

        final incomeData = snapshot.data ?? [];
        final rows = _buildRows(widget.selectedYear, incomeData);
        final profitIncrease = _calculateAverageMonthlyIncrease(rows);

        return Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: _IncomeDetailsTableCard(
                selectedYear: widget.selectedYear,
                rows: rows,
                onPrint: () {
                  _printIncomeReport(
                    year: widget.selectedYear,
                    rows: rows,
                    profitIncreasePercentage: profitIncrease,
                  );
                },
              ),
            ),
            const SizedBox(width: 18),
            _ProfitIncreaseCard(
              profitIncreasePercentage: profitIncrease,
            ),
          ],
        );
      },
    );
  }
}

class _IncomeDetailsTableCard extends StatelessWidget {
  final int selectedYear;
  final List<_IncomeRowData> rows;
  final VoidCallback onPrint;

  const _IncomeDetailsTableCard({
    required this.selectedYear,
    required this.rows,
    required this.onPrint,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: IncomeScreen.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Income Details - $selectedYear",
            style: const TextStyle(
              fontSize: 31,
              fontWeight: FontWeight.w800,
              color: IncomeScreen.textColor,
            ),
          ),
          const SizedBox(height: 12),
          Container(
            decoration: BoxDecoration(
              border: Border.all(color: IncomeScreen.borderColor),
            ),
            child: Column(
              children: [
                Container(
                  color: const Color(0xFFF9F9FB),
                  padding: const EdgeInsets.symmetric(vertical: 10),
                  child: const Row(
                    children: [
                      _TableCell(
                        text: "Month",
                        flex: 3,
                        isHeader: true,
                        hasRightBorder: true,
                      ),
                      _TableCell(
                        text: "Premium account income",
                        flex: 5,
                        isHeader: true,
                      ),
                    ],
                  ),
                ),
                ...rows.map(
                  (row) => Container(
                    decoration: const BoxDecoration(
                      border: Border(
                        top: BorderSide(color: IncomeScreen.borderColor),
                      ),
                    ),
                    child: Row(
                      children: [
                        _TableCell(
                          text: row.month,
                          flex: 3,
                          hasRightBorder: true,
                        ),
                        _TableCell(
                          text: row.isAvailable && row.premiumIncome != null
                              ? row.premiumIncome!.toStringAsFixed(2)
                              : "x",
                          flex: 5,
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 14),
          Align(
            alignment: Alignment.centerRight,
            child: SizedBox(
              width: 92,
              height: 34,
              child: ElevatedButton(
                onPressed: onPrint,
                style: ElevatedButton.styleFrom(
                  backgroundColor: IncomeScreen.darkPurple,
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(2),
                  ),
                  elevation: 0,
                ),
                child: const Text(
                  "Print",
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _TableCell extends StatelessWidget {
  final String text;
  final int flex;
  final bool isHeader;
  final bool hasRightBorder;

  const _TableCell({
    required this.text,
    required this.flex,
    this.isHeader = false,
    this.hasRightBorder = false,
  });

  @override
  Widget build(BuildContext context) {
    return Expanded(
      flex: flex,
      child: Container(
        height: 28,
        padding: const EdgeInsets.symmetric(horizontal: 8),
        alignment: Alignment.centerLeft,
        decoration: BoxDecoration(
          border: hasRightBorder
              ? const Border(
                  right: BorderSide(color: IncomeScreen.borderColor),
                )
              : null,
        ),
        child: Text(
          text,
          style: TextStyle(
            fontSize: 11,
            fontWeight: isHeader ? FontWeight.w600 : FontWeight.w400,
            color: IncomeScreen.textColor,
          ),
        ),
      ),
    );
  }
}

class _ProfitIncreaseCard extends StatelessWidget {
  final double profitIncreasePercentage;

  const _ProfitIncreaseCard({
    required this.profitIncreasePercentage,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 120,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: IncomeScreen.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Profit increase",
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: IncomeScreen.textColor,
            ),
          ),
          const SizedBox(height: 2),
          const Text(
            "per month",
            style: TextStyle(
              fontSize: 11,
              color: IncomeScreen.subTextColor,
            ),
          ),
          const SizedBox(height: 26),
          Center(
            child: Text(
              "${profitIncreasePercentage.toStringAsFixed(2)}%",
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w800,
                color: IncomeScreen.textColor,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _IncomeRowData {
  final String month;
  final double? premiumIncome;
  final bool isAvailable;

  _IncomeRowData({
    required this.month,
    required this.premiumIncome,
    required this.isAvailable,
  });
}