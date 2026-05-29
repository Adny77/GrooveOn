import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_stripe/flutter_stripe.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/providers/album_provider.dart';
import 'package:grooveon_mobile/providers/auth_provider.dart';
import 'package:grooveon_mobile/providers/music_search_provider.dart';
import 'package:grooveon_mobile/providers/notification_provider.dart';
import 'package:grooveon_mobile/providers/player_provider.dart';
import 'package:grooveon_mobile/providers/question_provider.dart';
import 'package:grooveon_mobile/providers/song_provider.dart';
import 'package:grooveon_mobile/providers/subscription_plan_provider.dart';
import 'package:grooveon_mobile/providers/subscription_provider.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/providers/payment_provider.dart';
import 'package:grooveon_mobile/routes/app_routes.dart';
import 'package:provider/provider.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await dotenv.load(fileName: ".env");
  await Session.load();

  Stripe.publishableKey = dotenv.get('STRIPE_PUBLISHABLE_KEY');

  await Stripe.instance.applySettings();

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(create: (_) => PlayerProvider()),
        ChangeNotifierProvider(create: (_) => SongProvider()),
        ChangeNotifierProvider(create: (_) => AlbumProvider()),
        ChangeNotifierProvider(create: (_) => MusicSearchProvider()),
        ChangeNotifierProvider(create: (_) => PaymentProvider()),
        ChangeNotifierProvider(create: (_) => SubscriptionProvider()),
        ChangeNotifierProvider(create: (_) => SubscriptionPlanProvider()),
        ChangeNotifierProvider(create: (_) => QuestionProvider()),
        ChangeNotifierProvider(create: (_) => NotificationProvider()),

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
      navigatorKey: HttpHelper.navigatorKey,
      debugShowCheckedModeBanner: false,
      title: 'GrooveOn',
      initialRoute: Session.isLoggedIn ? AppRoutes.navigation : AppRoutes.login,
      onGenerateRoute: AppRoutes.onGenerateRoute,
      theme: ThemeData(
        useMaterial3: true,
        scaffoldBackgroundColor: Colors.white,
        colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF9C27B0)),
      ),
    );
  }
}
