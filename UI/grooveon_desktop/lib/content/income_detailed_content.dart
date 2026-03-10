import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/income_screen.dart';

class IncomeDetailedContent extends StatelessWidget {
  const IncomeDetailedContent({super.key});

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: const [
        Expanded(
          child: _IncomeDetailsTableCard(),
        ),
        SizedBox(width: 18),
        _ProfitIncreaseCard(),
      ],
    );
  }
}

class _IncomeDetailsTableCard extends StatelessWidget {
  const _IncomeDetailsTableCard();

  @override
  Widget build(BuildContext context) {
    final months = [
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

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: IncomeScreen.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Income Details",
            style: TextStyle(
              fontSize: 31,
              fontWeight: FontWeight.w800,
              color: IncomeScreen.textColor,
            ),
          ),
          const SizedBox(height: 12),

          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const SizedBox(width: 180),
              Row(
                children: [
                  const Text(
                    "Select time interval:",
                    style: TextStyle(
                      fontSize: 12,
                      color: IncomeScreen.textColor,
                    ),
                  ),
                  const SizedBox(width: 8),
                  _FakeDateField(label: "From"),
                  const SizedBox(width: 8),
                  _FakeDateField(label: "To"),
                ],
              ),
            ],
          ),

          const SizedBox(height: 10),

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
                        flex: 2,
                        isHeader: true,
                        hasRightBorder: true,
                      ),
                      _TableCell(
                        text: "Premium account income",
                        flex: 3,
                        isHeader: true,
                        hasRightBorder: true,
                      ),
                      _TableCell(
                        text: "Paid promotions income",
                        flex: 3,
                        isHeader: true,
                        hasRightBorder: true,
                      ),
                      _TableCell(
                        text: "Paid ads income",
                        flex: 2,
                        isHeader: true,
                      ),
                    ],
                  ),
                ),
                ...months.map(
                  (month) => Container(
                    decoration: const BoxDecoration(
                      border: Border(
                        top: BorderSide(color: IncomeScreen.borderColor),
                      ),
                    ),
                    child: Row(
                      children: [
                        _TableCell(
                          text: month,
                          flex: 2,
                          hasRightBorder: true,
                        ),
                        const _TableCell(
                          text: "",
                          flex: 3,
                          hasRightBorder: true,
                        ),
                        const _TableCell(
                          text: "",
                          flex: 3,
                          hasRightBorder: true,
                        ),
                        const _TableCell(
                          text: "",
                          flex: 2,
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
                onPressed: () {},
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

class _FakeDateField extends StatelessWidget {
  final String label;

  const _FakeDateField({
    required this.label,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 88,
      height: 22,
      padding: const EdgeInsets.symmetric(horizontal: 6),
      decoration: BoxDecoration(
        color: const Color(0xFFF9F9FB),
        border: Border.all(color: IncomeScreen.borderColor),
      ),
      child: Row(
        children: [
          Expanded(
            child: Text(
              label,
              style: const TextStyle(
                fontSize: 10,
                color: IncomeScreen.subTextColor,
              ),
            ),
          ),
          const Icon(
            Icons.keyboard_arrow_down,
            size: 14,
            color: IncomeScreen.subTextColor,
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
  const _ProfitIncreaseCard();

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 120,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: IncomeScreen.borderColor),
      ),
      child: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Profit increase",
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.w700,
              color: IncomeScreen.textColor,
            ),
          ),
          SizedBox(height: 2),
          Text(
            "per month",
            style: TextStyle(
              fontSize: 11,
              color: IncomeScreen.subTextColor,
            ),
          ),
          SizedBox(height: 26),
          Center(
            child: Text(
              "xx.xx%",
              style: TextStyle(
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