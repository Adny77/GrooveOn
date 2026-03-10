import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/income_screen.dart';
import 'package:grooveon_desktop/screens/music_screen.dart';
import 'package:grooveon_desktop/screens/users_screen.dart';
import 'package:grooveon_desktop/widgets/sidebar_widget.dart';

class BaseScreen extends StatefulWidget {
  const BaseScreen({super.key});

  @override
  State<BaseScreen> createState() => _BaseScreenState();
}

class _BaseScreenState extends State<BaseScreen> {
  int _selectedIndex = 0;

  final List<Widget> _screens = const [
    MusicScreen(),
    UsersScreen(),
    IncomeScreen(),
  ];

  void _onMenuChanged(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F5F7),
      body: Row(
        children: [
          SidebarWidget(
            selectedIndex: _selectedIndex,
            onItemSelected: _onMenuChanged,
          ),
          Expanded(
            child: _screens[_selectedIndex],
          ),
        ],
      ),
    );
  }
}