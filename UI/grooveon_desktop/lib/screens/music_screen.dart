import 'package:flutter/material.dart';
import 'package:grooveon_desktop/content/music_add_content.dart';
import 'package:grooveon_desktop/content/music_delete_content.dart';
import 'package:grooveon_desktop/content/music_overview_content.dart';

enum MusicTab {
  overview,
  add,
  delete,
}

class MusicScreen extends StatefulWidget {
  const MusicScreen({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color primaryLight = Color(0xFFF7E9FB);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);
  static const Color dangerColor = Color(0xFFE53935);

  @override
  State<MusicScreen> createState() => _MusicScreenState();
}

class _MusicScreenState extends State<MusicScreen> {
  MusicTab _selectedTab = MusicTab.overview;

  void _changeTab(MusicTab tab) {
    setState(() {
      _selectedTab = tab;
    });
  }

  Widget _buildContent() {
    switch (_selectedTab) {
      case MusicTab.overview:
        return const MusicOverContent();
      case MusicTab.add:
        return const MusicAddContent();
      case MusicTab.delete:
        return const MusicDeleteContent();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _MusicTopTabs(
            selectedTab: _selectedTab,
            onTabChanged: _changeTab,
          ),
          const SizedBox(height: 18),
          Expanded(child: _buildContent()),
        ],
      ),
    );
  }
}

class _MusicTopTabs extends StatelessWidget {
  final MusicTab selectedTab;
  final ValueChanged<MusicTab> onTabChanged;

  const _MusicTopTabs({
    required this.selectedTab,
    required this.onTabChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _MusicTabItem(
          title: "Overview",
          active: selectedTab == MusicTab.overview,
          onTap: () => onTabChanged(MusicTab.overview),
        ),
        const SizedBox(width: 20),
        _MusicTabItem(
          title: "Add",
          active: selectedTab == MusicTab.add,
          onTap: () => onTabChanged(MusicTab.add),
        ),
        const SizedBox(width: 20),
        _MusicTabItem(
          title: "Delete",
          active: selectedTab == MusicTab.delete,
          activeColor: MusicScreen.dangerColor,
          onTap: () => onTabChanged(MusicTab.delete),
        ),
      ],
    );
  }
}

class _MusicTabItem extends StatelessWidget {
  final String title;
  final bool active;
  final VoidCallback onTap;
  final Color? activeColor;

  const _MusicTabItem({
    required this.title,
    required this.active,
    required this.onTap,
    this.activeColor,
  });

  @override
  Widget build(BuildContext context) {
    final color = activeColor ?? MusicScreen.primaryColor;

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.only(bottom: 4),
        decoration: BoxDecoration(
          border: active
              ? Border(
                  bottom: BorderSide(
                    color: color,
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
            color: active ? color : MusicScreen.textColor,
          ),
        ),
      ),
    );
  }
}