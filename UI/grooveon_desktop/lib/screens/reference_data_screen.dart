import 'package:flutter/material.dart';
import 'package:grooveon_desktop/content/artist_content.dart';
import 'package:grooveon_desktop/content/genre_content.dart';
import 'package:grooveon_desktop/content/role_content.dart';
import 'package:grooveon_desktop/content/subscription_plan_content.dart';

enum _RefTab { genres, roles, plans, artists }

class ReferenceDataScreen extends StatefulWidget {
  const ReferenceDataScreen({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color bgColor = Color(0xFFF5F5F7);
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  State<ReferenceDataScreen> createState() => _ReferenceDataScreenState();
}

class _ReferenceDataScreenState extends State<ReferenceDataScreen> {
  _RefTab _tab = _RefTab.genres;

  Widget _buildContent() {
    return switch (_tab) {
      _RefTab.genres => const GenreContent(),
      _RefTab.roles => const RoleContent(),
      _RefTab.plans => const SubscriptionPlanContent(),
      _RefTab.artists => const ArtistContent(),
    };
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: ReferenceDataScreen.bgColor,
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Reference Data',
            style: TextStyle(
              fontSize: 26,
              fontWeight: FontWeight.w800,
              color: ReferenceDataScreen.textColor,
            ),
          ),
          const SizedBox(height: 4),
          const Text(
            'Manage genres, roles and subscription plans.',
            style: TextStyle(
                fontSize: 13, color: ReferenceDataScreen.subTextColor),
          ),
          const SizedBox(height: 20),
          _TabBar(selected: _tab, onChanged: (t) => setState(() => _tab = t)),
          const SizedBox(height: 16),
          Expanded(child: _buildContent()),
        ],
      ),
    );
  }
}

class _TabBar extends StatelessWidget {
  final _RefTab selected;
  final void Function(_RefTab) onChanged;

  const _TabBar({required this.selected, required this.onChanged});

  static const Color _primary = Color(0xFF9C27B0);
  static const Color _border = Color(0xFFD9D9DE);

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        _tab(_RefTab.genres, 'Genres', Icons.music_note_outlined),
        const SizedBox(width: 8),
        _tab(_RefTab.roles, 'Roles', Icons.admin_panel_settings_outlined),
        const SizedBox(width: 8),
        _tab(_RefTab.plans, 'Subscription Plans',
            Icons.workspace_premium_outlined),
        const SizedBox(width: 8),
        _tab(_RefTab.artists, 'Artists', Icons.person_outlined),
      ],
    );
  }

  Widget _tab(_RefTab t, String label, IconData icon) {
    final active = selected == t;
    return OutlinedButton.icon(
      onPressed: () => onChanged(t),
      icon: Icon(icon, size: 16),
      label: Text(label),
      style: OutlinedButton.styleFrom(
        backgroundColor: active ? _primary : Colors.white,
        foregroundColor: active ? Colors.white : const Color(0xFF6F6F78),
        side: BorderSide(color: active ? _primary : _border),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        shape:
            RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      ),
    );
  }
}
