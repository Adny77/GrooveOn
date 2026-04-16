import 'package:flutter/material.dart';
import 'package:grooveon_desktop/providers/album_provider.dart';
import 'package:grooveon_desktop/providers/auth_provider.dart';
import 'package:grooveon_desktop/providers/report_provider.dart';
import 'package:grooveon_desktop/providers/song_provider.dart';
import 'package:grooveon_desktop/providers/user_provider.dart';
import 'package:grooveon_desktop/routes/app_routes.dart';
import 'package:grooveon_desktop/screens/login_screen.dart';
import 'package:provider/provider.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(create: (_) => ReportProvider()),
        ChangeNotifierProvider(create: (_) => SongProvider()),
        ChangeNotifierProvider(create: (_) => AlbumProvider()),
      ],
      child: const GrooveOnApp(),
    ),
  );
}

class GrooveOnApp extends StatelessWidget {
  const GrooveOnApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'GrooveOn',
      initialRoute: AppRoutes.login,
      onGenerateRoute: AppRoutes.onGenerateRoute,
      theme: ThemeData(
        useMaterial3: true,
        scaffoldBackgroundColor: Colors.white,
      ),
      home: const LoginScreen(),
    );
  }
}
