import 'package:flutter/material.dart';

class IncomeOverContent extends StatelessWidget {
  const IncomeOverContent({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color midPurple = Color(0xFF9C27B0);
  static const Color lightGray = Color(0xFFBDB8C7);
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
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: const [
          Text(
            "Yearly Income",
            style: TextStyle(
              fontSize: 31,
              fontWeight: FontWeight.w800,
              color: IncomeOverContent.textColor,
            ),
          ),
          SizedBox(height: 8),
          Expanded(
            child: _IncomeChartCard(),
          ),
        ],
      ),
    );
  }
}

class _IncomeChartCard extends StatelessWidget {
  const _IncomeChartCard();

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
        children: const [
          Expanded(
            child: _IncomeChartPainterWidget(),
          ),
          SizedBox(height: 8),
          _IncomeLegend(),
          SizedBox(height: 8),
        ],
      ),
    );
  }
}

class _IncomeChartPainterWidget extends StatelessWidget {
  const _IncomeChartPainterWidget();

  @override
  Widget build(BuildContext context) {
    return CustomPaint(
      painter: _IncomeChartPainter(),
      size: const Size(double.infinity, 420),
    );
  }
}

class _IncomeChartPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final gridPaint = Paint()
      ..color = const Color(0xFFD8D8DE)
      ..strokeWidth = 1;

    final axisTextStyle = const TextStyle(
      fontSize: 11,
      color: IncomeOverContent.subTextColor,
    );

    final premiumPaint = Paint()
      ..color = IncomeOverContent.lightGray
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    final promotionsPaint = Paint()
      ..color = IncomeOverContent.midPurple
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    final paidAdsPaint = Paint()
      ..color = IncomeOverContent.darkPurple
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    const leftPadding = 48.0;
    const bottomPadding = 35.0;
    const topPadding = 18.0;
    final chartWidth = size.width - leftPadding - 20;
    final chartHeight = size.height - topPadding - bottomPadding;

    for (int i = 0; i < 5; i++) {
      final y = topPadding + (i * chartHeight / 4);
      canvas.drawLine(
        Offset(leftPadding, y),
        Offset(leftPadding + chartWidth, y),
        gridPaint,
      );
    }

    const yLabels = ["12500", "10000", "7500", "5000", "2500"];
    for (int i = 0; i < yLabels.length; i++) {
      final tp = TextPainter(
        text: TextSpan(
          text: yLabels[i],
          style: axisTextStyle.copyWith(
            color: i == 0 || i == 1
                ? IncomeOverContent.midPurple
                : IncomeOverContent.darkPurple,
            fontWeight: FontWeight.w600,
          ),
        ),
        textDirection: TextDirection.ltr,
      )..layout();
      tp.paint(
        canvas,
        Offset(4, topPadding + (i * chartHeight / 4) - 6),
      );
    }

    const months = [
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
      "Dec"
    ];

    for (int i = 0; i < months.length; i++) {
      final x = leftPadding + (i * chartWidth / 11);
      final tp = TextPainter(
        text: TextSpan(
          text: months[i],
          style: axisTextStyle.copyWith(
            color: i == 0 ? IncomeOverContent.midPurple : IncomeOverContent.darkPurple,
            fontWeight: FontWeight.w600,
          ),
        ),
        textDirection: TextDirection.ltr,
      )..layout();
      tp.paint(canvas, Offset(x - 8, size.height - 22));
    }

    List<Offset> makePoints(List<double> values) {
      return List.generate(values.length, (i) {
        final x = leftPadding + (i * chartWidth / (values.length - 1));
        final y = topPadding + ((12500 - values[i]) / 10000) * chartHeight;
        return Offset(x, y);
      });
    }

    final premiumPoints = makePoints([
      9800, 8600, 6900, 10100, 7800, 6900, 6700, 9200, 8500, 8900, 7600, 6900
    ]);

    final promotionsPoints = makePoints([
      6400, 3200, 9700, 5400, 5400, 7600, 1600, 9400, 8900, 7800, 5400, 2600
    ]);

    final paidAdsPoints = makePoints([
      6600, 8400, 4700, 8300, 6800, 7000, 6400, 7600, 5800, 6300, 8200, 6500
    ]);

    void drawLine(List<Offset> points, Paint paint) {
      final path = Path()..moveTo(points.first.dx, points.first.dy);
      for (final point in points.skip(1)) {
        path.lineTo(point.dx, point.dy);
      }
      canvas.drawPath(path, paint);

      for (final point in points) {
        canvas.drawCircle(point, 3.5, Paint()..color = paint.color);
      }
    }

    drawLine(premiumPoints, premiumPaint);
    drawLine(promotionsPoints, promotionsPaint);
    drawLine(paidAdsPoints, paidAdsPaint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _IncomeLegend extends StatelessWidget {
  const _IncomeLegend();

  @override
  Widget build(BuildContext context) {
    return const Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        _IncomeLegendItem(
          color: IncomeOverContent.lightGray,
          title: "Premium Accounts",
        ),
        SizedBox(width: 32),
        _IncomeLegendItem(
          color: IncomeOverContent.midPurple,
          title: "Promotions",
        ),
        SizedBox(width: 32),
        _IncomeLegendItem(
          color: IncomeOverContent.darkPurple,
          title: "Paid Ads",
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