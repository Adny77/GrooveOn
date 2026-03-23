import 'dart:convert';

import 'package:grooveon_desktop/deezer/models/deezer_album.dart';
import 'package:grooveon_desktop/deezer/models/deezer_album_details.dart';
import 'package:grooveon_desktop/deezer/models/deezer_response.dart';
import 'package:grooveon_desktop/deezer/models/deezer_track.dart';
import 'package:http/http.dart' as http;

class DeezerProvider {
  static const String _baseUrl = 'https://api.deezer.com';

  Future<DeezerResponse<DeezerTrack>> searchTracks({
    required String query,
    int limit = 10,
    int index = 0,
  }) async {
    final uri = Uri.parse(
      '$_baseUrl/search?q=${Uri.encodeQueryComponent(query)}&limit=$limit&index=$index',
    );

    final response = await http.get(uri);

    _throwIfFailed(response, 'Greška pri pretrazi pjesama.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerTrack>.fromJson(
      jsonMap,
      (item) => DeezerTrack.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<DeezerResponse<DeezerAlbum>> searchAlbums({
    required String query,
    int limit = 10,
    int index = 0,
  }) async {
    final uri = Uri.parse(
      '$_baseUrl/search/album?q=${Uri.encodeQueryComponent(query)}&limit=$limit&index=$index',
    );

    final response = await http.get(uri);

    _throwIfFailed(response, 'Greška pri pretrazi albuma.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerAlbum>.fromJson(
      jsonMap,
      (item) => DeezerAlbum.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<DeezerResponse<DeezerTrack>> getTopTracks({
    int limit = 5,
    int index = 0,
  }) async {
    final uri = Uri.parse(
      '$_baseUrl/chart/0/tracks?limit=$limit&index=$index',
    );

    final response = await http.get(uri);

    _throwIfFailed(response, 'Greška pri dohvaćanju top pjesama.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerTrack>.fromJson(
      jsonMap,
      (item) => DeezerTrack.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<DeezerAlbumDetails> getAlbumDetails(int albumId) async {
    final uri = Uri.parse('$_baseUrl/album/$albumId');

    final response = await http.get(uri);

    _throwIfFailed(response, 'Greška pri dohvaćanju detalja albuma.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerAlbumDetails.fromJson(jsonMap);
  }

  Future<DeezerResponse<DeezerTrack>> getTracksFromNextUrl(String nextUrl) async {
    final response = await http.get(Uri.parse(nextUrl));

    _throwIfFailed(response, 'Greška pri dohvaćanju naredne stranice pjesama.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerTrack>.fromJson(
      jsonMap,
      (item) => DeezerTrack.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<DeezerResponse<DeezerAlbum>> getAlbumsFromNextUrl(String nextUrl) async {
    final response = await http.get(Uri.parse(nextUrl));

    _throwIfFailed(response, 'Greška pri dohvaćanju naredne stranice albuma.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerAlbum>.fromJson(
      jsonMap,
      (item) => DeezerAlbum.fromJson(item as Map<String, dynamic>),
    );
  }

  void _throwIfFailed(http.Response response, String message) {
    if (response.statusCode != 200) {
      throw Exception('$message Status code: ${response.statusCode}');
    }
  }
}