import 'dart:convert';
import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/models/song_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class SongProvider extends BaseProvider<SongResponse> {
  SongProvider() : super("Song");

  @override
  SongResponse fromJson(data) {
    return SongResponse.fromJson(data);
  }

  Future<List<SongResponse>> getRecommended({int take = 4}) async {
  final uri = Uri.parse(
    '${ApiConfig.apiBase}/api/Song/recommended?take=$take',
  );

  final response = await http.get(
    uri,
    headers: HttpHelper.getHeaders(),
  );

  HttpHelper.checkResponse(response);

  final data = jsonDecode(response.body) as List<dynamic>;

  return data
      .map((x) => SongResponse.fromJson(x as Map<String, dynamic>))
      .toList();
}
}