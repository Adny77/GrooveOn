import 'package:flutter_dotenv/flutter_dotenv.dart';

class ApiConfig {
  static String get apiBase => dotenv.get('API_BASE_URL_MOBILE');

  static String get imagesUsers => "$apiBase/images/users";
  static String get imagesProperties => "$apiBase/images/playlists";

  static Map<String, String> imageFolders = {
    'users': imagesUsers,
    'playlists': imagesProperties,
  };
}