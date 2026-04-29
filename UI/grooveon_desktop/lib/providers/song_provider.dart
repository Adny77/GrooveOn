import 'dart:convert';

import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/helper/http_helper.dart';
import 'package:grooveon_desktop/models/request/song_bulk_insert_request.dart';
import 'package:grooveon_desktop/models/request/song_duplicate_check_request.dart';
import 'package:grooveon_desktop/models/response/search_result.dart';
import 'package:grooveon_desktop/models/response/song_bulk_insert_response.dart';
import 'package:grooveon_desktop/models/response/song_duplicate_check_response.dart';
import 'package:grooveon_desktop/models/response/song_response.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class SongProvider extends BaseProvider<SongResponse> {
  SongProvider() : super("Song");

  @override
  SongResponse fromJson(data) {
    return SongResponse.fromJson(data);
  }

  Future<SearchResult<SongResponse>> getPaged({
    required int page,
    required int pageSize,
    String? filter,
    bool includeTotalCount = true,
  }) async {
    final Map<String, dynamic> filterMap = {
      "page": page,
      "pageSize": pageSize,
      "includeArtist": true,
      "includeAlbum": true,
      "includeTotalCount": includeTotalCount,
    };

    if (filter != null && filter.trim().isNotEmpty) {
      filterMap["FTS"] = filter.trim();
    }

    return await get(filter: filterMap);
  }

  Future<SongDuplicateCheckResponse> checkDuplicates(
    SongDuplicateCheckRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Song/check-duplicates";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return SongDuplicateCheckResponse.fromJson(jsonMap);
  }

  Future<SongBulkInsertResponse> bulkSaveDeezerSongs(
    SongBulkInsertRequest request,
  ) async {
    final url = "${ApiConfig.apiBase}/api/Song/bulk-save-deezer";

    final response = await http.post(
      Uri.parse(url),
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request.toJson()),
    );

    HttpHelper.checkResponse(response);

    final jsonMap = jsonDecode(response.body) as Map<String, dynamic>;
    return SongBulkInsertResponse.fromJson(jsonMap);
  }
}