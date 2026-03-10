import 'package:flutter/material.dart';
import 'package:grooveon_desktop/screens/login_screen.dart';

class AppRoutes {
  static const String login = '/login';
  static const String base = '/base';

  static Route<dynamic>? onGenerateRoute(RouteSettings settings) {
    switch (settings.name) {

      case login:
        return MaterialPageRoute(
          builder: (_) => const LoginScreen(),
        );
      
      default:
        return MaterialPageRoute(
          builder: (_) => const LoginScreen(),
        );
    }
  }
}
