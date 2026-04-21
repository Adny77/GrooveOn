import 'dart:convert';
import 'package:grooveon_mobile/models/album_response.dart';
import 'package:grooveon_mobile/models/search_results.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';
import 'package:http/http.dart' as http;

class AlbumProvider extends BaseProvider<AlbumResponse> {
  AlbumProvider() : super("Album");

  @override
  AlbumResponse fromJson(data) {
    return AlbumResponse.fromJson(data);
  }
}