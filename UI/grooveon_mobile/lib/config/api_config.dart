import 'package:flutter_dotenv/flutter_dotenv.dart';

class ApiConfig {
  //static String get apiBase => dotenv.env['API_BASE_URL_MOBILE'] ?? "";
  static String get apiBase => "http://10.0.2.2:5201";

  static String imagesUsers = "$apiBase/images/users";
  static String imagesProperties = "$apiBase/images/playlists";

  static Map<String, String> imageFolders = {
    'users': imagesUsers,
    'playlists': imagesProperties,
  };
}