import 'package:flutter_dotenv/flutter_dotenv.dart';

class ApiConfig {
  //static String get apiBase => dotenv.env['API_BASE_URL_MOBILE'] ?? "";
  static String get apiBase => "http://10.0.2.2:5201";
}