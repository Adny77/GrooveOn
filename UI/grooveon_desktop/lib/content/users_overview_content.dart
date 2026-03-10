import 'package:flutter/material.dart';

class UsersOverContent extends StatelessWidget {
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
  Widget build(BuildContext context) {
    return Container(
      color: bgColor,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: const [
                  _UsersAnalyticsSection(),
                ],
              ),
            ),
          ),
          const SizedBox(width: 18),
          const _AverageUseTimeCard(),
        ],
      ),
    );
  }
}

class _UsersAnalyticsSection extends StatelessWidget {
  const _UsersAnalyticsSection();

  @override
  Widget build(BuildContext context) {
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
            border: Border.all(color: UsersOverContent.borderColor),
          ),
          child: Column(
            children: const [
              Row(
                children: [
                  Expanded(
                    child: _UsersSwitchHeader(
                      title: "Year",
                      active: true,
                    ),
                  ),
                  Expanded(
                    child: _UsersSwitchHeader(
                      title: "Month",
                      active: false,
                    ),
                  ),
                ],
              ),
              Padding(
                padding: EdgeInsets.all(18),
                child: _UsersDonutSection(),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),
        const Text(
          "Use time overview",
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.w700,
            color: UsersOverContent.textColor,
          ),
        ),
        const SizedBox(height: 8),
        const _UseTimeOverviewCard(),
      ],
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
                  color:
                      active ? UsersOverContent.primaryColor : UsersOverContent.textColor,
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
  const _UsersDonutSection();

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 240,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: const [
          Expanded(
            flex: 2,
            child: _UsersLegend(),
          ),
          Expanded(
            flex: 4,
            child: Center(
              child: _DonutPlaceholder(),
            ),
          ),
          Expanded(
            flex: 2,
            child: _DonutPercentages(),
          ),
        ],
      ),
    );
  }
}

class _UsersLegend extends StatelessWidget {
  const _UsersLegend();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 28),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: const [
          _LegendItem(
            color: UsersOverContent.darkPurple,
            title: "Basic Accounts",
          ),
          SizedBox(height: 10),
          _LegendItem(
            color: UsersOverContent.lightPurple,
            title: "Premium Accounts",
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
        Text(
          title,
          style: const TextStyle(
            fontSize: 12,
            color: UsersOverContent.textColor,
          ),
        ),
      ],
    );
  }
}

class _DonutPlaceholder extends StatelessWidget {
  const _DonutPlaceholder();

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 210,
      height: 210,
      child: CustomPaint(
        painter: _DonutPainter(),
      ),
    );
  }
}

class _DonutPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final strokeWidth = 48.0;
    final center = Offset(size.width / 2, size.height / 2);
    final radius = (size.width / 2) - 18;

    final rect = Rect.fromCircle(center: center, radius: radius);

    final paint1 = Paint()
      ..color = UsersOverContent.darkPurple
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth;

    final paint2 = Paint()
      ..color = UsersOverContent.lightPurple
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth;

    canvas.drawArc(
      rect,
      -1.2,
      5.62,
      false,
      paint1,
    );

    canvas.drawArc(
      rect,
      4.42,
      0.66,
      false,
      paint2,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _DonutPercentages extends StatelessWidget {
  const _DonutPercentages();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 48),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _PercentageLabel(
            value: "10.56%",
            topMargin: 0,
          ),
          const SizedBox(height: 54),
          _PercentageLabel(
            value: "89.44%",
            topMargin: 0,
          ),
        ],
      ),
    );
  }
}

class _PercentageLabel extends StatelessWidget {
  final String value;
  final double topMargin;

  const _PercentageLabel({
    required this.value,
    required this.topMargin,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.only(top: topMargin),
      child: Row(
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
      ),
    );
  }
}

class _AverageUseTimeCard extends StatelessWidget {
  const _AverageUseTimeCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 150,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 18),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersOverContent.borderColor),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Average use time",
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: UsersOverContent.textColor,
            ),
          ),
          SizedBox(height: 2),
          Text(
            "per day",
            style: TextStyle(
              fontSize: 12,
              color: UsersOverContent.subTextColor,
            ),
          ),
          SizedBox(height: 24),
          Center(
            child: Text(
              "2.5hr",
              style: TextStyle(
                fontSize: 18,
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

class _UseTimeOverviewCard extends StatelessWidget {
  const _UseTimeOverviewCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 210,
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: UsersOverContent.borderColor),
      ),
      child: CustomPaint(
        painter: _UseTimePainter(),
        size: const Size(double.infinity, 180),
      ),
    );
  }
}

class _UseTimePainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final gridPaint = Paint()
      ..color = const Color(0xFFD8D8DE)
      ..strokeWidth = 1;

    final linePaint = Paint()
      ..color = UsersOverContent.lightPurple
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    final fillPaint = Paint()
      ..color = UsersOverContent.lightPurple.withOpacity(0.18)
      ..style = PaintingStyle.fill;

    const leftPadding = 20.0;
    const rightPadding = 20.0;
    const topPadding = 20.0;
    const bottomPadding = 20.0;

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

    final values = <double>[
      58, 42, 66, 53, 58, 51, 45,
      39, 57, 55, 59, 56, 44, 66,
      52, 64,
    ];

    final minValue = 30.0;
    final maxValue = 80.0;

    final points = List.generate(values.length, (index) {
      final x = leftPadding + (index * chartWidth / (values.length - 1));
      final normalized = (values[index] - minValue) / (maxValue - minValue);
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
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}