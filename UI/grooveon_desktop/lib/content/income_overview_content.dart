import 'package:flutter/material.dart';
import 'package:grooveon_desktop/models/income_by_month_response.dart';
import 'package:grooveon_desktop/providers/report_provider.dart';
import 'package:provider/provider.dart';

class IncomeOverContent extends StatefulWidget {
  final int selectedYear;
  final ValueChanged<int> onYearChanged;

  const IncomeOverContent({
    super.key,
    required this.selectedYear,
    required this.onYearChanged,
  });

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFBDB8C7);
  static const Color bgColor = Color(0xFFF5F5F7);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  State<IncomeOverContent> createState() => _IncomeOverContentState();
}

class _IncomeOverContentState extends State<IncomeOverContent> {
  late Future<List<IncomeByMonthResponse>> _futureIncome;

  @override
  void initState() {
    super.initState();
    _futureIncome = _loadIncome();
  }

  @override
  void didUpdateWidget(covariant IncomeOverContent oldWidget) {
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

  @override
  Widget build(BuildContext context) {
    return Container(
      color: IncomeOverContent.bgColor,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Yearly Income (KM)",
            style: TextStyle(
              fontSize: 31,
              fontWeight: FontWeight.w800,
              color: IncomeOverContent.textColor,
            ),
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10),
                decoration: BoxDecoration(
                  color: Colors.white,
                  border: Border.all(color: IncomeOverContent.borderColor),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: DropdownButton<int>(
                  value: widget.selectedYear,
                  underline: const SizedBox(),
                  borderRadius: BorderRadius.circular(12),
                  items: const [
                    DropdownMenuItem(
                      value: 2025,
                      child: Text("2025"),
                    ),
                    DropdownMenuItem(
                      value: 2026,
                      child: Text("2026"),
                    ),
                  ],
                  onChanged: (value) {
                    if (value == null) return;
                    widget.onYearChanged(value);
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Expanded(
            child: FutureBuilder<List<IncomeByMonthResponse>>(
              future: _futureIncome,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return Container(
                    width: double.infinity,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      border: Border.all(
                        color: IncomeOverContent.borderColor,
                      ),
                    ),
                    child: const Center(
                      child: CircularProgressIndicator(),
                    ),
                  );
                }

                if (snapshot.hasError) {
                  return Container(
                    width: double.infinity,
                    padding: const EdgeInsets.all(24),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      border: Border.all(
                        color: IncomeOverContent.borderColor,
                      ),
                    ),
                    child: Text(
                      "Greška pri učitavanju income podataka: ${snapshot.error}",
                      style: const TextStyle(
                        color: Colors.red,
                        fontSize: 14,
                      ),
                    ),
                  );
                }

                final data = snapshot.data ?? [];

                return _IncomeChartCard(
                  data: data,
                  selectedYear: widget.selectedYear,
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}

class _IncomeChartCard extends StatefulWidget {
  final List<IncomeByMonthResponse> data;
  final int selectedYear;

  const _IncomeChartCard({
    required this.data,
    required this.selectedYear,
  });

  @override
  State<_IncomeChartCard> createState() => _IncomeChartCardState();
}

class _IncomeChartCardState extends State<_IncomeChartCard> {
  int? _hoveredIndex;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.fromLTRB(18, 18, 18, 10),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: IncomeOverContent.borderColor),
      ),
      child: Column(
        children: [
          Expanded(
            child: LayoutBuilder(
              builder: (context, constraints) {
                final chartSize = Size(
                  constraints.maxWidth,
                  constraints.maxHeight - 24,
                );

                return MouseRegion(
                  onHover: (event) {
                    final index = _findClosestIndex(
                      localPosition: event.localPosition,
                      size: chartSize,
                      data: widget.data,
                      selectedYear: widget.selectedYear,
                    );

                    if (_hoveredIndex != index) {
                      setState(() {
                        _hoveredIndex = index;
                      });
                    }
                  },
                  onExit: (_) {
                    if (_hoveredIndex != null) {
                      setState(() {
                        _hoveredIndex = null;
                      });
                    }
                  },
                  child: CustomPaint(
                    size: chartSize,
                    painter: _IncomeChartPainter(
                      data: widget.data,
                      hoveredIndex: _hoveredIndex,
                      selectedYear: widget.selectedYear,
                    ),
                  ),
                );
              },
            ),
          ),
          const SizedBox(height: 8),
          const _IncomeLegend(),
          const SizedBox(height: 8),
        ],
      ),
    );
  }

  int? _findClosestIndex({
    required Offset localPosition,
    required Size size,
    required List<IncomeByMonthResponse> data,
    required int selectedYear,
  }) {
    final values = _normalizeMonthlyValues(data, selectedYear);
    if (values.isEmpty) return null;

    const leftPadding = 52.0;
    const rightPadding = 20.0;
    const topPadding = 18.0;
    const bottomPadding = 35.0;

    final chartWidth = size.width - leftPadding - rightPadding;
    final chartHeight = size.height - topPadding - bottomPadding;

    if (chartWidth <= 0 || chartHeight <= 0) return null;

    final maxValue = values.reduce((a, b) => a > b ? a : b);
    final upperBound = _niceUpperBound(maxValue);

    final points = List.generate(values.length, (index) {
      final x = values.length == 1
          ? leftPadding + chartWidth / 2
          : leftPadding + (index * chartWidth / (values.length - 1));

      final normalized = upperBound == 0 ? 0.0 : values[index] / upperBound;
      final y = topPadding + chartHeight - (normalized * chartHeight);
      return Offset(x, y);
    });

    int? closestIndex;
    double closestDistance = double.infinity;

    for (int i = 0; i < points.length; i++) {
      final distance = (points[i] - localPosition).distance;
      if (distance < closestDistance) {
        closestDistance = distance;
        closestIndex = i;
      }
    }

    return closestDistance <= 18 ? closestIndex : null;
  }

  List<double> _normalizeMonthlyValues(
    List<IncomeByMonthResponse> data,
    int selectedYear,
  ) {
    final maxMonth = _maxAllowedMonthForYear(selectedYear);
    if (maxMonth <= 0) return [];

    final values = List<double>.filled(maxMonth, 0);

    for (final item in data) {
      if (item.month >= 1 && item.month <= maxMonth) {
        values[item.month - 1] = item.totalIncome;
      }
    }

    return values;
  }

  int _maxAllowedMonthForYear(int year) {
    final now = DateTime.now();

    if (year < now.year) return 12;
    if (year == now.year) return now.month - 1;
    return 0;
  }

  double _niceUpperBound(double maxValue) {
    if (maxValue <= 0) return 10;
    if (maxValue <= 10) return 10;
    if (maxValue <= 20) return 20;
    if (maxValue <= 50) return 50;
    if (maxValue <= 100) return 100;
    if (maxValue <= 200) return 200;
    if (maxValue <= 500) return 500;
    if (maxValue <= 1000) return 1000;
    return (maxValue / 100).ceil() * 100.0;
  }
}

class _IncomeChartPainter extends CustomPainter {
  final List<IncomeByMonthResponse> data;
  final int? hoveredIndex;
  final int selectedYear;

  _IncomeChartPainter({
    required this.data,
    required this.hoveredIndex,
    required this.selectedYear,
  });

  @override
  void paint(Canvas canvas, Size size) {
    final values = _normalizeMonthlyValues(data, selectedYear);
    final months = _visibleMonths(selectedYear);

    if (values.isEmpty || months.isEmpty) {
      _drawEmptyState(canvas, size);
      return;
    }

    final gridPaint = Paint()
      ..color = const Color(0xFFD8D8DE)
      ..strokeWidth = 1;

    final axisTextStyle = const TextStyle(
      fontSize: 11,
      color: IncomeOverContent.subTextColor,
    );

    final linePaint = Paint()
      ..color = IncomeOverContent.darkPurple
      ..strokeWidth = 2.2
      ..style = PaintingStyle.stroke;

    const leftPadding = 52.0;
    const bottomPadding = 35.0;
    const topPadding = 18.0;
    const rightPadding = 20.0;

    final chartWidth = size.width - leftPadding - rightPadding;
    final chartHeight = size.height - topPadding - bottomPadding;

    for (int i = 0; i < 5; i++) {
      final y = topPadding + (i * chartHeight / 4);
      canvas.drawLine(
        Offset(leftPadding, y),
        Offset(leftPadding + chartWidth, y),
        gridPaint,
      );
    }

    final maxValue = values.reduce((a, b) => a > b ? a : b);
    final upperBound = _niceUpperBound(maxValue);

    for (int i = 0; i < 5; i++) {
      final y = topPadding + (i * chartHeight / 4);
      final value = ((4 - i) * upperBound / 4);

      final tp = TextPainter(
        text: TextSpan(
          text: value.toStringAsFixed(value % 1 == 0 ? 0 : 2),
          style: axisTextStyle.copyWith(
            color: IncomeOverContent.darkPurple,
            fontWeight: FontWeight.w600,
          ),
        ),
        textDirection: TextDirection.ltr,
      )..layout();

      tp.paint(canvas, Offset(4, y - 6));
    }

    for (int i = 0; i < months.length; i++) {
      final x = months.length == 1
          ? leftPadding + chartWidth / 2
          : leftPadding + (i * chartWidth / (months.length - 1));

      final tp = TextPainter(
        text: TextSpan(
          text: months[i],
          style: axisTextStyle.copyWith(
            color: IncomeOverContent.darkPurple,
            fontWeight: FontWeight.w600,
          ),
        ),
        textDirection: TextDirection.ltr,
      )..layout();

      tp.paint(canvas, Offset(x - (tp.width / 2), size.height - 22));
    }

    final points = List.generate(values.length, (i) {
      final x = values.length == 1
          ? leftPadding + chartWidth / 2
          : leftPadding + (i * chartWidth / (values.length - 1));

      final normalized = upperBound == 0 ? 0.0 : values[i] / upperBound;
      final y = topPadding + chartHeight - (normalized * chartHeight);
      return Offset(x, y);
    });

    final path = Path()..moveTo(points.first.dx, points.first.dy);
    for (final point in points.skip(1)) {
      path.lineTo(point.dx, point.dy);
    }
    canvas.drawPath(path, linePaint);

    for (int i = 0; i < points.length; i++) {
      final isHovered = hoveredIndex == i;

      if (isHovered) {
        canvas.drawCircle(
          points[i],
          8,
          Paint()..color = IncomeOverContent.primaryColor.withOpacity(0.18),
        );
      }

      canvas.drawCircle(
        points[i],
        isHovered ? 4.8 : 3.5,
        Paint()..color = IncomeOverContent.darkPurple,
      );
    }

    if (hoveredIndex != null &&
        hoveredIndex! >= 0 &&
        hoveredIndex! < values.length) {
      _drawTooltip(
        canvas,
        point: points[hoveredIndex!],
        label: values[hoveredIndex!].toStringAsFixed(2),
        size: size,
      );
    }
  }

  void _drawEmptyState(Canvas canvas, Size size) {
    final tp = TextPainter(
      text: const TextSpan(
        text: "Nema dostupnih podataka za prikaz",
        style: TextStyle(
          color: IncomeOverContent.subTextColor,
          fontSize: 14,
          fontWeight: FontWeight.w500,
        ),
      ),
      textDirection: TextDirection.ltr,
    )..layout();

    tp.paint(
      canvas,
      Offset(
        (size.width - tp.width) / 2,
        (size.height - tp.height) / 2,
      ),
    );
  }

  void _drawTooltip(
    Canvas canvas, {
    required Offset point,
    required String label,
    required Size size,
  }) {
    final textPainter = TextPainter(
      text: TextSpan(
        text: label,
        style: const TextStyle(
          color: Colors.white,
          fontSize: 11,
          fontWeight: FontWeight.w700,
        ),
      ),
      textDirection: TextDirection.ltr,
    )..layout();

    const horizontalPadding = 10.0;
    const verticalPadding = 6.0;

    final tooltipWidth = textPainter.width + horizontalPadding * 2;
    final tooltipHeight = textPainter.height + verticalPadding * 2;

    double dx = point.dx - tooltipWidth / 2;
    double dy = point.dy - tooltipHeight - 12;

    if (dx < 0) dx = 0;
    if (dx + tooltipWidth > size.width) {
      dx = size.width - tooltipWidth;
    }
    if (dy < 0) {
      dy = point.dy + 12;
    }

    final rect = RRect.fromRectAndRadius(
      Rect.fromLTWH(dx, dy, tooltipWidth, tooltipHeight),
      const Radius.circular(8),
    );

    canvas.drawRRect(
      rect,
      Paint()..color = const Color(0xFF2A2A2A),
    );

    textPainter.paint(
      canvas,
      Offset(dx + horizontalPadding, dy + verticalPadding),
    );
  }

  List<double> _normalizeMonthlyValues(
    List<IncomeByMonthResponse> data,
    int selectedYear,
  ) {
    final maxMonth = _maxAllowedMonthForYear(selectedYear);
    if (maxMonth <= 0) return [];

    final values = List<double>.filled(maxMonth, 0);

    for (final item in data) {
      if (item.month >= 1 && item.month <= maxMonth) {
        values[item.month - 1] = item.totalIncome;
      }
    }

    return values;
  }

  List<String> _visibleMonths(int year) {
    const allMonths = [
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec",
    ];

    final maxMonth = _maxAllowedMonthForYear(year);
    if (maxMonth <= 0) return [];

    return allMonths.take(maxMonth).toList();
  }

  int _maxAllowedMonthForYear(int year) {
    final now = DateTime.now();

    if (year < now.year) return 12;
    if (year == now.year) return now.month - 1;
    return 0;
  }

  double _niceUpperBound(double maxValue) {
    if (maxValue <= 0) return 10;
    if (maxValue <= 10) return 10;
    if (maxValue <= 20) return 20;
    if (maxValue <= 50) return 50;
    if (maxValue <= 100) return 100;
    if (maxValue <= 200) return 200;
    if (maxValue <= 500) return 500;
    if (maxValue <= 1000) return 1000;
    return (maxValue / 100).ceil() * 100.0;
  }

  @override
  bool shouldRepaint(covariant _IncomeChartPainter oldDelegate) {
    return oldDelegate.data != data ||
        oldDelegate.hoveredIndex != hoveredIndex ||
        oldDelegate.selectedYear != selectedYear;
  }
}

class _IncomeLegend extends StatelessWidget {
  const _IncomeLegend();

  @override
  Widget build(BuildContext context) {
    return const Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        _IncomeLegendItem(
          color: IncomeOverContent.darkPurple,
          title: "Premium Accounts",
        ),
      ],
    );
  }
}

class _IncomeLegendItem extends StatelessWidget {
  final Color color;
  final String title;

  const _IncomeLegendItem({
    required this.color,
    required this.title,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 8,
          height: 8,
          decoration: BoxDecoration(
            color: color,
            shape: BoxShape.circle,
          ),
        ),
        const SizedBox(width: 8),
        Text(
          title,
          style: const TextStyle(
            fontSize: 12,
            color: IncomeOverContent.textColor,
          ),
        ),
      ],
    );
  }
}