import 'package:flutter/material.dart';
import 'package:grooveon_desktop/content/income_detailed_content.dart';
import 'package:grooveon_desktop/content/income_overview_content.dart';

enum IncomeTab {
  overview,
  detailedView,
}

class IncomeScreen extends StatefulWidget {
  const IncomeScreen({super.key});

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
  State<IncomeScreen> createState() => _IncomeScreenState();
}

class _IncomeScreenState extends State<IncomeScreen> {
  IncomeTab _selectedTab = IncomeTab.overview;
  int _selectedYear = 2026;

  void _changeTab(IncomeTab tab) {
    setState(() {
      _selectedTab = tab;
    });
  }

  void _changeSelectedYear(int year) {
    setState(() {
      _selectedYear = year;
    });
  }

  Widget _buildContent() {
    switch (_selectedTab) {
      case IncomeTab.overview:
        return IncomeOverContent(
          selectedYear: _selectedYear,
          onYearChanged: _changeSelectedYear,
        );
      case IncomeTab.detailedView:
        return IncomeDetailedContent(
          selectedYear: _selectedYear,
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: IncomeScreen.bgColor,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _IncomeTopTabs(
            selectedTab: _selectedTab,
            onTabChanged: _changeTab,
          ),
          const SizedBox(height: 18),
          Expanded(
            child: _buildContent(),
          ),
        ],
      ),
    );
  }
}

class _IncomeTopTabs extends StatelessWidget {
  final IncomeTab selectedTab;
  final ValueChanged<IncomeTab> onTabChanged;

  const _IncomeTopTabs({
    required this.selectedTab,
    required this.onTabChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _IncomeTabItem(
          title: "Overview",
          active: selectedTab == IncomeTab.overview,
          onTap: () => onTabChanged(IncomeTab.overview),
        ),
        const SizedBox(width: 20),
        _IncomeTabItem(
          title: "Detailed view",
          active: selectedTab == IncomeTab.detailedView,
          onTap: () => onTabChanged(IncomeTab.detailedView),
        ),
      ],
    );
  }
}

class _IncomeTabItem extends StatelessWidget {
  final String title;
  final bool active;
  final VoidCallback onTap;

  const _IncomeTabItem({
    required this.title,
    required this.active,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.only(bottom: 4),
        decoration: BoxDecoration(
          border: active
              ? const Border(
                  bottom: BorderSide(
                    color: IncomeScreen.primaryColor,
                    width: 2,
                  ),
                )
              : null,
        ),
        child: Text(
          title,
          style: TextStyle(
            fontSize: 13,
            fontWeight: active ? FontWeight.w700 : FontWeight.w500,
            color: active ? IncomeScreen.primaryColor : IncomeScreen.textColor,
          ),
        ),
      ),
    );
  }
}