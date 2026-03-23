import 'dart:convert';

import 'package:grooveon_desktop/models/request/album_upsert_request.dart';
import 'package:grooveon_desktop/models/response/album_preview_response.dart';
import 'package:grooveon_desktop/models/response/album_response.dart';
import 'package:grooveon_desktop/models/response/album_save_response.dart';
import 'package:http/http.dart' as http;
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';
import 'package:grooveon_desktop/utils/session.dart';

class AlbumProvider extends BaseProvider<AlbumResponse> {
  AlbumProvider() : super("Album");

  @override
  AlbumResponse fromJson(data) {
    return AlbumResponse.fromJson(data);
  }

  Future<AlbumPreviewResponse> previewDeezerAlbum(
    AlbumUpsertRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Album/preview-deezer";

    final response = await http.post(
      Uri.parse(url),
      headers: _createHeaders(),
      body: jsonEncode(request.toJson()),
    );

    _throwIfNotSuccess(response, "Greška pri preview-u albuma.");

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return AlbumPreviewResponse.fromJson(jsonMap);
  }

  Future<AlbumSaveResponse> saveDeezerAlbum(
    AlbumUpsertRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Album/save-deezer";

    final response = await http.post(
      Uri.parse(url),
      headers: _createHeaders(),
      body: jsonEncode(request.toJson()),
    );

    _throwIfNotSuccess(response, "Greška pri spašavanju albuma.");

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return AlbumSaveResponse.fromJson(jsonMap);
  }

  Map<String, String> _createHeaders() {
    return {
      "Content-Type": "application/json",
      "Accept": "application/json",
      if (Session.token != null && Session.token!.isNotEmpty)
        "Authorization": "Bearer ${Session.token}",
    };
  }

  void _throwIfNotSuccess(http.Response response, String message) {
    if (response.statusCode < 200 || response.statusCode > 299) {
      throw Exception("$message ${response.statusCode}: ${response.body}");
    }
  }
}