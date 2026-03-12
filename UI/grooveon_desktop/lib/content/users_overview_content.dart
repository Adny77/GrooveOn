import 'package:flutter/material.dart';
import 'package:grooveon_desktop/models/subscription_analytics.dart';
import 'package:grooveon_desktop/models/user_growth_point.dart';
import 'package:grooveon_desktop/providers/report_provider.dart';
import 'package:provider/provider.dart';

class UsersOverContent extends StatefulWidget {
  const UsersOverContent({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFAD2DBF);
  static const Color bgColor = Color(0xFFF5F5F7);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  State<UsersOverContent> createState() => _UsersOverContentState();
}

class _UsersOverContentState extends State<UsersOverContent> {
  bool _yearMode = true;
  int _selectedGrowthYear = 2026;

  late Future<SubscriptionAnalytics> _futureAnalytics;
  late Future<List<UserGrowthPoint>> _futureGrowth;

  @override
  void initState() {
    super.initState();
    _futureAnalytics = _loadAnalytics();
    _futureGrowth = _loadGrowth();
  }

  Future<SubscriptionAnalytics> _loadAnalytics() {
    final provider = context.read<ReportProvider>();

    return provider.getAnalytics(
      year: 2026,
      month: _yearMode ? null : DateTime.now().month,
    );
  }

  Future<List<UserGrowthPoint>> _loadGrowth() {
    final provider = context.read<ReportProvider>();
    return provider.getUserGrowthByMonth(year: _selectedGrowthYear);
  }

  void _changeMode(bool yearMode) {
    if (_yearMode == yearMode) return;

    setState(() {
      _yearMode = yearMode;
      _futureAnalytics = _loadAnalytics();
    });
  }

  void _changeGrowthYear(int year) {
    if (_selectedGrowthYear == year) return;

    setState(() {
      _selectedGrowthYear = year;
      _futureGrowth = _loadGrowth();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: UsersOverContent.bgColor,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  FutureBuilder<SubscriptionAnalytics>(
                    future: _futureAnalytics,
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return Container(
                          height: 320,
                          decoration: BoxDecoration(
                            color: UsersOverContent.cardColor,
                            border: Border.all(
                              color: UsersOverContent.borderColor,
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
                            color: UsersOverContent.cardColor,
                            border: Border.all(
                              color: UsersOverContent.borderColor,
                            ),
                          ),
                          child: Text(
                            "Greška pri učitavanju podataka: ${snapshot.error}",
                            style: const TextStyle(
                              color: Colors.red,
                              fontSize: 14,
                            ),
                          ),
                        );
                      }

                      final data = snapshot.data!;

                      return Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            "New Users",
                            style: TextStyle(
                              fontSize: 31,
                              fontWeight: FontWeight.w800,
                              color: UsersOverContent.textColor,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Container(
                            decoration: BoxDecoration(
                              color: UsersOverContent.cardColor,
                              border: Border.all(
                                color: UsersOverContent.borderColor,
                              ),
                            ),
                            child: Column(
                              children: [
                                Row(
                                  children: [
                                    Expanded(
                                      child: InkWell(
                                        onTap: () => _changeMode(true),
                                        child: _UsersSwitchHeader(
                                          title: "Year",
                                          active: _yearMode,
                                        ),
                                      ),
                                    ),
                                    Expanded(
                                      child: InkWell(
                                        onTap: () => _changeMode(false),
                                        child: _UsersSwitchHeader(
                                          title: "Month",
                                          active: !_yearMode,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                                Padding(
                                  padding: const EdgeInsets.all(18),
                                  child: _UsersDonutSection(data: data),
                                ),
                              ],
                            ),
                          ),
                        ],
                      );
                    },
                  ),
                  const SizedBox(height: 18),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        "User growth overview",
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w700,
                          color: UsersOverContent.textColor,
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10),
                        decoration: BoxDecoration(
                          color: Colors.white,
                          border: Border.all(
                            color: UsersOverContent.borderColor,
                          ),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: DropdownButton<int>(
                          value: _selectedGrowthYear,
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
                            _changeGrowthYear(value);
                          },
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  FutureBuilder<List<UserGrowthPoint>>(
                    future: _futureGrowth,
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return Container(
                          height: 210,
                          width: double.infinity,
                          decoration: BoxDecoration(
                            color: Colors.white,
                            border: Border.all(
                              color: UsersOverContent.borderColor,
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
                              color: UsersOverContent.borderColor,
                            ),
                          ),
                          child: Text(
                            "Greška pri učitavanju rasta korisnika: ${snapshot.error}",
                            style: const TextStyle(
                              color: Colors.red,
                              fontSize: 14,
                            ),
                          ),
                        );
                      }

                      final growthData = snapshot.data ?? [];

                      return _UserGrowthOverviewCard(
                        data: growthData,
                      );
                    },
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(width: 18),
          FutureBuilder<List<UserGrowthPoint>>(
            future: _futureGrowth,
            builder: (context, snapshot) {
              final growthData = snapshot.data ?? const <UserGrowthPoint>[];
              return _NewUsersAverageCard(data: growthData);
            },
          ),
        ],
      ),
    );
  }
}

class _UsersSwitchHeader extends StatelessWidget {
  final String title;
  final bool active;

  const _UsersSwitchHeader({
    required this.title,
    required this.active,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 34,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: active ? Colors.white : const Color(0xFFF2F2F4),
        border: const Border(
          bottom: BorderSide(color: UsersOverContent.borderColor),
        ),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          if (active)
            Container(
              height: 2,
              width: double.infinity,
              color: UsersOverContent.primaryColor,
            ),
          Expanded(
            child: Center(
              child: Text(
                title,
                style: TextStyle(
                  fontSize: 13,
                  fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                  color: active
                      ? UsersOverContent.primaryColor
                      : UsersOverContent.textColor,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _UsersDonutSection extends StatelessWidget {
  final SubscriptionAnalytics data;

  const _UsersDonutSection({
    required this.data,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 240,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            flex: 2,
            child: _UsersLegend(
              basicCount: data.basicCount,
              premiumCount: data.premiumCount,
            ),
          ),
          Expanded(
            flex: 4,
            child: Center(
              child: _DonutChart(
                basicPercentage: data.basicPercentage,
                premiumPercentage: data.premiumPercentage,
              ),
            ),
          ),
          Expanded(
            flex: 2,
            child: _DonutPercentages(
              basicPercentage: data.basicPercentage,
              premiumPercentage: data.premiumPercentage,
            ),
          ),
        ],
      ),
    );
  }
}

class _UsersLegend extends StatelessWidget {
  final int basicCount;
  final int premiumCount;

  const _UsersLegend({
    required this.basicCount,
    required this.premiumCount,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 28),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _LegendItem(
            color: UsersOverContent.darkPurple,
            title: "Basic Accounts ($basicCount)",
          ),
          const SizedBox(height: 10),
          _LegendItem(
            color: UsersOverContent.lightPurple,
            title: "Premium Accounts ($premiumCount)",
          ),
        ],
      ),
    );
  }
}

class _LegendItem extends StatelessWidget {
  final Color color;
  final String title;

  const _LegendItem({
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
        Expanded(
          child: Text(
            title,
            style: const TextStyle(
              fontSize: 12,
              color: UsersOverContent.textColor,
            ),
          ),
        ),
      ],
    );
  }
}

class _DonutChart extends StatelessWidget {
  final double basicPercentage;
  final double premiumPercentage;

  const _DonutChart({
    required this.basicPercentage,
    required this.premiumPercentage,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 210,
      height: 210,
      child: CustomPaint(
        painter: _DonutPainter(
          basicPercentage: basicPercentage,
          premiumPercentage: premiumPercentage,
        ),
      ),
    );
  }
}

class _DonutPainter extends CustomPainter {
  final double basicPercentage;
  final double premiumPercentage;

  _DonutPainter({
    required this.basicPercentage,
    required this.premiumPercentage,
  });

  @override
  void paint(Canvas canvas, Size size) {
    const strokeWidth = 48.0;
    final center = Offset(size.width / 2, size.height / 2);
    final radius = (size.width / 2) - 18;
    final rect = Rect.fromCircle(center: center, radius: radius);

    final backgroundPaint = Paint()
      ..color = const Color(0xFFE9E9EE)
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth
      ..strokeCap = StrokeCap.butt;

    final basicPaint = Paint()
      ..color = UsersOverContent.darkPurple
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth
      ..strokeCap = StrokeCap.butt;

    final premiumPaint = Paint()
      ..color = UsersOverContent.lightPurple
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth
      ..strokeCap = StrokeCap.butt;

    canvas.drawArc(rect, -1.5708, 6.28318, false, backgroundPaint);

    final basicSweep = 6.28318 * (basicPercentage / 100);
    final premiumSweep = 6.28318 * (premiumPercentage / 100);

    double startAngle = -1.5708;

    if (basicSweep > 0) {
      canvas.drawArc(rect, startAngle, basicSweep, false, basicPaint);
      startAngle += basicSweep;
    }

    if (premiumSweep > 0) {
      canvas.drawArc(rect, startAngle, premiumSweep, false, premiumPaint);
    }
  }

  @override
  bool shouldRepaint(covariant _DonutPainter oldDelegate) {
    return oldDelegate.basicPercentage != basicPercentage ||
        oldDelegate.premiumPercentage != premiumPercentage;
  }
}

class _DonutPercentages extends StatelessWidget {
  final double basicPercentage;
  final double premiumPercentage;

  const _DonutPercentages({
    required this.basicPercentage,
    required this.premiumPercentage,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 48),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _PercentageLabel(
            value: "${basicPercentage.toStringAsFixed(2)}%",
          ),
          const SizedBox(height: 54),
          _PercentageLabel(
            value: "${premiumPercentage.toStringAsFixed(2)}%",
          ),
        ],
      ),
    );
  }
}

class _PercentageLabel extends StatelessWidget {
  final String value;

  const _PercentageLabel({
    required this.value,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 22,
          height: 1,
          color: UsersOverContent.subTextColor,
        ),
        const SizedBox(width: 6),
        Text(
          value,
          style: const TextStyle(
            fontSize: 12,
            color: UsersOverContent.textColor,
          ),
        ),
      ],
    );
  }
}

class _NewUsersAverageCard extends StatelessWidget {
  final List<UserGrowthPoint> data;

  const _NewUsersAverageCard({
    required this.data,
  });

  double _averagePerMonth() {
    if (data.isEmpty) return 0;
    final total = data.fold<int>(0, (sum, item) => sum + item.count);
    return total / data.length;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 170,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 18),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersOverContent.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Average new users",
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: UsersOverContent.textColor,
            ),
          ),
          const SizedBox(height: 2),
          const Text(
            "per month",
            style: TextStyle(
              fontSize: 12,
              color: UsersOverContent.subTextColor,
            ),
          ),
          const SizedBox(height: 24),
          Center(
            child: Text(
              _averagePerMonth().toStringAsFixed(1),
              style: const TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.w800,
                color: UsersOverContent.textColor,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _UserGrowthOverviewCard extends StatefulWidget {
  final List<UserGrowthPoint> data;

  const _UserGrowthOverviewCard({
    required this.data,
  });

  @override
  State<_UserGrowthOverviewCard> createState() => _UserGrowthOverviewCardState();
}

class _UserGrowthOverviewCardState extends State<_UserGrowthOverviewCard> {
  int? _hoveredIndex;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 230,
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersOverContent.borderColor),
      ),
      child: LayoutBuilder(
        builder: (context, constraints) {
          final chartSize = Size(constraints.maxWidth, 180);

          return MouseRegion(
            onHover: (event) {
              final index = _findClosestIndex(
                localPosition: event.localPosition,
                size: chartSize,
                data: widget.data,
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
              painter: _UserGrowthPainter(
                widget.data,
                hoveredIndex: _hoveredIndex,
              ),
            ),
          );
        },
      ),
    );
  }

  int? _findClosestIndex({
    required Offset localPosition,
    required Size size,
    required List<UserGrowthPoint> data,
  }) {
    if (data.isEmpty) return null;

    const leftPadding = 38.0;
    const rightPadding = 12.0;
    const topPadding = 18.0;
    const bottomPadding = 34.0;

    final chartWidth = size.width - leftPadding - rightPadding;
    final chartHeight = size.height - topPadding - bottomPadding;

    if (chartWidth <= 0 || chartHeight <= 0) return null;

    final values = data.map((e) => e.count.toDouble()).toList();
    final maxValue = values.reduce((a, b) => a > b ? a : b);
    final upperBound = _niceUpperBound(maxValue);

    final points = List.generate(data.length, (index) {
      final x = data.length == 1
          ? leftPadding + (chartWidth / 2)
          : leftPadding + (index * chartWidth / (data.length - 1));

      final normalized = values[index] / upperBound;
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

  double _niceUpperBound(double maxValue) {
    if (maxValue <= 5) return 5;
    if (maxValue <= 10) return 10;
    if (maxValue <= 20) return 20;
    if (maxValue <= 50) return 50;
    return (maxValue / 10).ceil() * 10.0;
  }
}

class _UserGrowthPainter extends CustomPainter {
  final List<UserGrowthPoint> data;
  final int? hoveredIndex;

  _UserGrowthPainter(
    this.data, {
    this.hoveredIndex,
  });

  @override
  void paint(Canvas canvas, Size size) {
    final gridPaint = Paint()
      ..color = const Color(0xFFD8D8DE)
      ..strokeWidth = 1;

    final linePaint = Paint()
      ..color = UsersOverContent.lightPurple
      ..strokeWidth = 2.2
      ..style = PaintingStyle.stroke;

    final fillPaint = Paint()
      ..color = UsersOverContent.lightPurple.withOpacity(0.15)
      ..style = PaintingStyle.fill;

    final labelStyle = const TextStyle(
      color: UsersOverContent.subTextColor,
      fontSize: 11,
      fontWeight: FontWeight.w500,
    );

    const leftPadding = 38.0;
    const rightPadding = 12.0;
    const topPadding = 18.0;
    const bottomPadding = 34.0;

    final chartWidth = size.width - leftPadding - rightPadding;
    final chartHeight = size.height - topPadding - bottomPadding;

    for (int i = 0; i < 5; i++) {
      final y = topPadding + (i * chartHeight / 4);
      canvas.drawLine(
        Offset(leftPadding, y),
        Offset(size.width - rightPadding, y),
        gridPaint,
      );
    }

    if (data.isEmpty) return;

    final values = data.map((e) => e.count.toDouble()).toList();
    final maxValue = values.reduce((a, b) => a > b ? a : b);
    final upperBound = _niceUpperBound(maxValue);

    for (int i = 0; i < 5; i++) {
      final y = topPadding + (i * chartHeight / 4);
      final value = ((4 - i) * upperBound / 4).round();

      final textPainter = TextPainter(
        text: TextSpan(
          text: value.toString(),
          style: labelStyle,
        ),
        textDirection: TextDirection.ltr,
      )..layout();

      textPainter.paint(
        canvas,
        Offset(leftPadding - textPainter.width - 8, y - textPainter.height / 2),
      );
    }

    final points = List.generate(data.length, (index) {
      final x = data.length == 1
          ? leftPadding + (chartWidth / 2)
          : leftPadding + (index * chartWidth / (data.length - 1));

      final normalized = values[index] / upperBound;
      final y = topPadding + chartHeight - (normalized * chartHeight);
      return Offset(x, y);
    });

    final path = Path()..moveTo(points.first.dx, points.first.dy);
    for (final point in points.skip(1)) {
      path.lineTo(point.dx, point.dy);
    }

    final fillPath = Path.from(path)
      ..lineTo(points.last.dx, size.height - bottomPadding)
      ..lineTo(points.first.dx, size.height - bottomPadding)
      ..close();

    canvas.drawPath(fillPath, fillPaint);
    canvas.drawPath(path, linePaint);

    for (int i = 0; i < points.length; i++) {
      final isHovered = hoveredIndex == i;

      if (isHovered) {
        canvas.drawCircle(
          points[i],
          8,
          Paint()..color = UsersOverContent.primaryColor.withOpacity(0.18),
        );
      }

      canvas.drawCircle(
        points[i],
        isHovered ? 4.5 : 3,
        Paint()..color = UsersOverContent.primaryColor,
      );

      final textPainter = TextPainter(
        text: TextSpan(
          text: data[i].label,
          style: labelStyle,
        ),
        textDirection: TextDirection.ltr,
      )..layout();

      textPainter.paint(
        canvas,
        Offset(
          points[i].dx - textPainter.width / 2,
          size.height - bottomPadding + 8,
        ),
      );
    }

    if (hoveredIndex != null &&
        hoveredIndex! >= 0 &&
        hoveredIndex! < data.length) {
      _drawTooltip(
        canvas,
        point: points[hoveredIndex!],
        label: "${data[hoveredIndex!].count}",
        size: size,
      );
    }
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

  double _niceUpperBound(double maxValue) {
    if (maxValue <= 5) return 5;
    if (maxValue <= 10) return 10;
    if (maxValue <= 20) return 20;
    if (maxValue <= 50) return 50;
    return (maxValue / 10).ceil() * 10.0;
  }

  @override
  bool shouldRepaint(covariant _UserGrowthPainter oldDelegate) {
    return oldDelegate.data != data ||
        oldDelegate.hoveredIndex != hoveredIndex;
  }
}