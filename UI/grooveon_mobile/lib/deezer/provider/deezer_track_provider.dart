import 'dart:convert';
import 'package:http/http.dart' as http;

class DeezerTrackProvider {
  static const String _baseUrl = "https://api.deezer.com/track";

  static Future<String?> getPreviewUrl(String externalTrackId) async {
    try {
      final url = Uri.parse("$_baseUrl/$externalTrackId");

      final response = await http.get(url);

      if (response.statusCode != 200) {
        return null;
      }

      final data = jsonDecode(response.body);

      final preview = data["preview"];

      if (preview == null || preview.toString().trim().isEmpty) {
        return null;
      }

      return preview;
    } catch (e) {
      return null;
    }
  }
}