import 'dart:convert';

import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:grooveon_desktop/models/request/album_upsert_request.dart';
import 'package:grooveon_desktop/models/response/album_preview_response.dart';
import 'package:grooveon_desktop/models/response/album_response.dart';
import 'package:grooveon_desktop/models/response/album_save_response.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class AlbumProvider extends BaseProvider<AlbumResponse> {
  AlbumProvider() : super("Album");

  @override
  AlbumResponse fromJson(data) {
    return AlbumResponse.fromJson(data);
  }

  Future<SearchResult<AlbumResponse>> getPaged({
    required int page,
    required int pageSize,
    String? filter,
    bool includeTotalCount = true,
  }) async {
    final Map<String, dynamic> filterMap = {
      "page": page,
      "pageSize": pageSize,
      "includeArtist": true,
      "includeTotalCount": includeTotalCount,
    };

    if (filter != null && filter.trim().isNotEmpty) {
      filterMap["FTS"] = filter.trim();
    }

    return await get(filter: filterMap);
  }

  Future<AlbumPreviewResponse> previewDeezerAlbum(
    AlbumUpsertRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Album/preview-deezer";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return AlbumPreviewResponse.fromJson(jsonMap);
  }

  Future<AlbumSaveResponse> saveDeezerAlbum(
    AlbumUpsertRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Album/save-deezer";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return AlbumSaveResponse.fromJson(jsonMap);
  }
}