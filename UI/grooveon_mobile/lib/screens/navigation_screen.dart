import 'package:flutter/material.dart';
import 'package:grooveon_mobile/screens/home_screen.dart';
import 'package:grooveon_mobile/screens/my_playlist_screen.dart';
import 'package:grooveon_mobile/screens/public_playlist_screen.dart';
import 'package:grooveon_mobile/screens/subscription_screen.dart';
import 'package:grooveon_mobile/screens/user_screen.dart';
import 'package:grooveon_mobile/widgets/mini_player_bar.dart';

class MainNavigationScreen extends StatefulWidget {
  const MainNavigationScreen({super.key});

  @override
  State<MainNavigationScreen> createState() => _MainNavigationScreenState();
}

class _MainNavigationScreenState extends State<MainNavigationScreen> {
  static const Color primary = Color(0xFF9C27B0);

  int _selectedIndex = 0;

  final GlobalKey<UserScreenState> _userScreenKey =
      GlobalKey<UserScreenState>();

  late final List<Widget> _screens = [
    const HomeScreen(showBottomNav: false),
    UserScreen(key: _userScreenKey),
    const PublicPlaylistsScreen(),
    const MyPlaylistsScreen(),
    const SubscriptionScreen(),
  ];

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
          _navItem(Icons.favorite_border_rounded, 2),
          _navItem(Icons.grid_view_rounded, 3),
          _navItem(Icons.workspace_premium_rounded, 4),
        ],
      ),
    );
  }

  Widget _navItem(IconData icon, int index) {
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
        child: Icon(
          icon,
          color: active ? Colors.white : Colors.black54,
        ),
      ),
    );
  }
}