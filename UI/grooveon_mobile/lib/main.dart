import 'package:flutter/material.dart';
import 'package:grooveon_mobile/providers/album_provider.dart';
import 'package:grooveon_mobile/providers/auth_provider.dart';
import 'package:grooveon_mobile/providers/music_search_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/providers/song_provider.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/routes/app_routes.dart';
import 'package:intl/date_symbol_data_file.dart';
import 'package:provider/provider.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(create: (_) => PlayerProvider()),
        ChangeNotifierProvider(create: (_) => SongProvider()),
        ChangeNotifierProvider(create: (_) => AlbumProvider()),
        ChangeNotifierProvider(create: (_) => MusicSearchProvider()),

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
        colorScheme: ColorScheme.fromSeed(
          seedColor: const Color(0xFF9C27B0),
        ),
      ),
    );
  }
}