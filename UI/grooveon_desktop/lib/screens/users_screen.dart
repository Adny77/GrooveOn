import 'package:flutter/material.dart';
import 'package:grooveon_desktop/content/users_overview_content.dart';
import 'package:grooveon_desktop/content/users_qa_content.dart';
import 'package:grooveon_desktop/content/users_userData_content.dart';

enum UsersTab {
  overview,
  userData,
  qa,
}

class UsersScreen extends StatefulWidget {
  const UsersScreen({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFAD2DBF);
  static const Color bgColor = Color(0xFFF5F5F7);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  State<UsersScreen> createState() => _UsersScreenState();
}

class _UsersScreenState extends State<UsersScreen> {
  UsersTab _selectedTab = UsersTab.overview;

  void _changeTab(UsersTab tab) {
    setState(() {
      _selectedTab = tab;
    });
  }

  Widget _buildContent() {
    switch (_selectedTab) {
      case UsersTab.overview:
        return const UsersOverContent();
      case UsersTab.userData:
        return const UsersDataContent();
      case UsersTab.qa:
        return const UsersQaContent();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: UsersScreen.bgColor,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _UsersTopTabs(
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

class _UsersTopTabs extends StatelessWidget {
  final UsersTab selectedTab;
  final ValueChanged<UsersTab> onTabChanged;

  const _UsersTopTabs({
    required this.selectedTab,
    required this.onTabChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _UsersTabItem(
          title: "Overview",
          active: selectedTab == UsersTab.overview,
          onTap: () => onTabChanged(UsersTab.overview),
        ),
        const SizedBox(width: 20),
        _UsersTabItem(
          title: "User data",
          active: selectedTab == UsersTab.userData,
          onTap: () => onTabChanged(UsersTab.userData),
        ),
        const SizedBox(width: 20),
        _UsersTabItem(
          title: "Q&A",
          active: selectedTab == UsersTab.qa,
          onTap: () => onTabChanged(UsersTab.qa),
        ),
      ],
    );
  }
}

class _UsersTabItem extends StatelessWidget {
  final String title;
  final bool active;
  final VoidCallback onTap;

  const _UsersTabItem({
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
                    color: UsersScreen.primaryColor,
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
            color: active ? UsersScreen.primaryColor : UsersScreen.textColor,
          ),
        ),
      ),
    );
  }
}