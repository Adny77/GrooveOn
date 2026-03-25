import 'dart:convert';

import 'package:grooveon_desktop/deezer/models/deezer_album.dart';
import 'package:grooveon_desktop/deezer/models/deezer_album_details.dart';
import 'package:grooveon_desktop/deezer/models/deezer_response.dart';
import 'package:grooveon_desktop/deezer/models/deezer_track.dart';
import 'package:http/http.dart' as http;

class DeezerProvider {
  static const String _baseUrl = 'https://api.deezer.com';

  final Map<int, DeezerAlbumDetails> _albumDetailsCache = {};

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

  Future<DeezerResponse<DeezerTrack>> searchTracksWithFullAlbums({
    required String query,
    int limit = 10,
    int index = 0,
  }) async {
    final result = await searchTracks(
      query: query,
      limit: limit,
      index: index,
    );

    final enrichedTracks = await _attachAlbumDetailsToTracks(result.data);

    return DeezerResponse<DeezerTrack>(
      data: enrichedTracks,
      total: result.total,
      next: result.next,
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

  Future<DeezerResponse<DeezerTrack>> getTopTracksWithFullAlbums({
    int limit = 5,
    int index = 0,
  }) async {
    final result = await getTopTracks(
      limit: limit,
      index: index,
    );

    final enrichedTracks = await _attachAlbumDetailsToTracks(result.data);

    return DeezerResponse<DeezerTrack>(
      data: enrichedTracks,
      total: result.total,
      next: result.next,
    );
  }

  Future<DeezerAlbumDetails> getAlbumDetails(int albumId) async {
    if (_albumDetailsCache.containsKey(albumId)) {
      return _albumDetailsCache[albumId]!;
    }

    final uri = Uri.parse('$_baseUrl/album/$albumId');

    final response = await http.get(uri);

    _throwIfFailed(response, 'Greška pri dohvaćanju detalja albuma.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    final details = DeezerAlbumDetails.fromJson(jsonMap);
    _albumDetailsCache[albumId] = details;

    return details;
  }

  Future<DeezerResponse<DeezerTrack>> getTracksFromNextUrl(
    String nextUrl,
  ) async {
    final response = await http.get(Uri.parse(nextUrl));

    _throwIfFailed(response, 'Greška pri dohvaćanju naredne stranice pjesama.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerTrack>.fromJson(
      jsonMap,
      (item) => DeezerTrack.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<DeezerResponse<DeezerTrack>> getTracksFromNextUrlWithFullAlbums(
    String nextUrl,
  ) async {
    final result = await getTracksFromNextUrl(nextUrl);

    final enrichedTracks = await _attachAlbumDetailsToTracks(result.data);

    return DeezerResponse<DeezerTrack>(
      data: enrichedTracks,
      total: result.total,
      next: result.next,
    );
  }

  Future<DeezerResponse<DeezerAlbum>> getAlbumsFromNextUrl(
    String nextUrl,
  ) async {
    final response = await http.get(Uri.parse(nextUrl));

    _throwIfFailed(response, 'Greška pri dohvaćanju naredne stranice albuma.');

    final Map<String, dynamic> jsonMap =
        jsonDecode(response.body) as Map<String, dynamic>;

    return DeezerResponse<DeezerAlbum>.fromJson(
      jsonMap,
      (item) => DeezerAlbum.fromJson(item as Map<String, dynamic>),
    );
  }

  Future<List<DeezerTrack>> _attachAlbumDetailsToTracks(
    List<DeezerTrack> tracks,
  ) async {
    final albumIds = tracks
        .map((t) => t.album?.id)
        .whereType<int>()
        .toSet()
        .toList();

    await Future.wait(
      albumIds.map((albumId) async {
        if (_albumDetailsCache.containsKey(albumId)) return;
        final details = await getAlbumDetails(albumId);
        _albumDetailsCache[albumId] = details;
      }),
    );

    return tracks.map((track) {
      final albumId = track.album?.id;

      if (albumId == null) {
        return track;
      }

      final fullAlbum = _albumDetailsCache[albumId];

      if (fullAlbum == null) {
        return track;
      }

      return track.copyWith(
        album: fullAlbum.toAlbum(),
      );
    }).toList();
  }

  void clearAlbumCache() {
    _albumDetailsCache.clear();
  }

  void _throwIfFailed(http.Response response, String message) {
    if (response.statusCode != 200) {
      throw Exception('$message Status code: ${response.statusCode}');
    }
  }
}