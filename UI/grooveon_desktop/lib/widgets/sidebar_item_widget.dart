import 'package:flutter/material.dart';
import 'package:grooveon_desktop/widgets/sidebar_widget.dart';

class SidebarItemWidget extends StatelessWidget {
  final IconData icon;
  final String title;
  final bool selected;
  final VoidCallback onTap;

  const SidebarItemWidget({
    super.key,
    required this.icon,
    required this.title,
    required this.selected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          borderRadius: BorderRadius.circular(12),
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
            decoration: BoxDecoration(
              color: selected
                  ? SidebarWidget.primaryColor.withOpacity(0.10)
                  : Colors.transparent,
              borderRadius: BorderRadius.circular(12),
              border: selected
                  ? const Border(
                      left: BorderSide(
                        color: SidebarWidget.primaryColor,
                        width: 3,
                      ),
                    )
                  : null,
            ),
            child: Row(
              children: [
                Icon(
                  icon,
                  size: 18,
                  color: selected
                      ? SidebarWidget.primaryColor
                      : SidebarWidget.textColor,
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Text(
                    title,
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
                      color: selected
                          ? SidebarWidget.primaryColor
                          : SidebarWidget.textColor,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}