import 'dart:async';

import 'package:flutter/material.dart';
import 'package:grooveon_mobile/providers/notification_provider.dart';
import 'package:grooveon_mobile/screens/explore_playlists_screen.dart';
import 'package:grooveon_mobile/screens/home_screen.dart';
import 'package:grooveon_mobile/screens/my_playlist_screen.dart';
import 'package:grooveon_mobile/screens/notifications_screen.dart';
import 'package:grooveon_mobile/screens/subscription_screen.dart';
import 'package:grooveon_mobile/screens/user_screen.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';
import 'package:provider/provider.dart';

class MainNavigationScreen extends StatefulWidget {
  const MainNavigationScreen({super.key});

  @override
  State<MainNavigationScreen> createState() => _MainNavigationScreenState();
}

class _MainNavigationScreenState extends State<MainNavigationScreen> {
  static const Color primary = Color(0xFF9C27B0);

  int _selectedIndex = 0;
  int _unreadNotificationCount = 0;
  Timer? _notificationTimer;

  final GlobalKey<UserScreenState> _userScreenKey =
      GlobalKey<UserScreenState>();

  late final List<Widget> _screens = [
    const HomeScreen(showBottomNav: false),
    UserScreen(key: _userScreenKey),
    const ExplorePlaylistsScreen(),
    const MyPlaylistsScreen(),
    const SubscriptionScreen(),
    NotificationsScreen(onUnreadChanged: _setUnreadNotificationCount),
  ];

  @override
  void initState() {
    super.initState();
    _loadUnreadNotifications();
    _notificationTimer = Timer.periodic(
      const Duration(seconds: 30),
      (_) => _loadUnreadNotifications(),
    );
  }

  @override
  void dispose() {
    _notificationTimer?.cancel();
    super.dispose();
  }

  void _setUnreadNotificationCount(int count) {
    if (!mounted || _unreadNotificationCount == count) return;

    setState(() {
      _unreadNotificationCount = count;
    });
  }

  Future<void> _loadUnreadNotifications() async {
    try {
      final result = await context.read<NotificationProvider>().get(filter: {
        "IsRead": false,
        "Page": 0,
        "PageSize": 20,
      });

      _setUnreadNotificationCount(result.items.length);
    } catch (_) {
      // Badge refresh should never interrupt navigation.
    }
  }

  Future<void> _changeTab(int index) async {
    if (_selectedIndex == index) return;

    if (_selectedIndex == 1) {
      final canLeave =
          await _userScreenKey.currentState?.confirmLeaveScreen() ?? true;

      if (!canLeave) return;
    }

    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        alignment: Alignment.bottomCenter,
        children: [
          IndexedStack(
            index: _selectedIndex,
            children: _screens,
          ),
          const MiniPlayerBar(),
        ],
      ),
      bottomNavigationBar: _bottomNav(),
    );
  }

  Widget _bottomNav() {
    return Container(
      height: 70,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
        boxShadow: [
          BoxShadow(
            color: Color(0x11000000),
            blurRadius: 10,
          ),
        ],
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _navItem(Icons.home_rounded, 0),
          _navItem(Icons.person_outline_rounded, 1),
          _navItem(Icons.explore_rounded, 2),
          _navItem(Icons.grid_view_rounded, 3),
          _navItem(Icons.workspace_premium_rounded, 4),
          _navItem(
            Icons.notifications_rounded,
            5,
            showBadge: _unreadNotificationCount > 0,
          ),
        ],
      ),
    );
  }

  Widget _navItem(IconData icon, int index, {bool showBadge = false}) {
    final active = _selectedIndex == index;

    return InkWell(
      borderRadius: BorderRadius.circular(30),
      onTap: () => _changeTab(index),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 180),
        padding: const EdgeInsets.all(10),
        decoration: active
            ? const BoxDecoration(
                color: primary,
                shape: BoxShape.circle,
              )
            : null,
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            Icon(
              icon,
              color: active ? Colors.white : Colors.black54,
            ),
            if (showBadge)
              Positioned(
                right: -1,
                top: -1,
                child: Container(
                  width: 9,
                  height: 9,
                  decoration: BoxDecoration(
                    color: active ? Colors.white : const Color(0xFFE53935),
                    shape: BoxShape.circle,
                    border: Border.all(
                      color: active ? primary : Colors.white,
                      width: 1.5,
                    ),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
