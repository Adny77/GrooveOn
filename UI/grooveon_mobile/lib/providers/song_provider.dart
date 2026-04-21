import 'dart:convert';
import 'package:grooveon_mobile/models/search_results.dart';
import 'package:grooveon_mobile/models/song_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class SongProvider extends BaseProvider<SongResponse> {
  SongProvider() : super("Song");

  @override
  SongResponse fromJson(data) {
    return SongResponse.fromJson(data);
  }
}