import 'package:flutter/foundation.dart';
import 'package:grooveon_desktop/deezer/models/deezer_album.dart';
import 'package:grooveon_desktop/deezer/models/deezer_album_details.dart';
import 'package:grooveon_desktop/deezer/models/deezer_track.dart';
import 'package:grooveon_desktop/deezer/provider/deezer_provider.dart';

class DeezerMusicHelper extends ChangeNotifier {
  final DeezerProvider _provider = DeezerProvider();

  DeezerProvider get provider => _provider;

  List<DeezerTrack> songResults = [];
  List<DeezerAlbum> albumResults = [];

  bool isLoadingSongs = false;
  bool isLoadingAlbums = false;
  bool isLoadingAlbumDetails = false;

  String? songNextUrl;
  String? albumNextUrl;

  String? songError;
  String? albumError;
  String? albumDetailsError;

  String? currentSongQuery;
  String? currentAlbumQuery;

  Future<void> loadInitialTopTracks() async {
  isLoadingSongs = true;
  songError = null;
  notifyListeners();

  try {
    final result = await _provider.searchTracksWithFullAlbums(
      query: "top",
      limit: 5,
      index: 0,
    );

    songResults = result.data;
    songNextUrl = result.next;
  } catch (e) {
    songError = e.toString();
  } finally {
    isLoadingSongs = false;
    notifyListeners();
  }
}

  Future<void> searchSongs(String query) async {
    final trimmed = query.trim();
    currentSongQuery = trimmed;

    if (trimmed.isEmpty) {
      await loadInitialTopTracks();
      return;
    }

    isLoadingSongs = true;
    songError = null;
    notifyListeners();

    try {
      final result = await _provider.searchTracksWithFullAlbums(
        query: trimmed,
        limit: 5,
        index: 0,
      );

      songResults = result.data;
      songNextUrl = result.next;
    } catch (e) {
      songError = e.toString();
    } finally {
      isLoadingSongs = false;
      notifyListeners();
    }
  }

  Future<void> loadMoreSongs() async {
    if (songNextUrl == null || isLoadingSongs) return;

    isLoadingSongs = true;
    notifyListeners();

    try {
      final result = await _provider.getTracksFromNextUrlWithFullAlbums(
        songNextUrl!,
      );

      songResults = [...songResults, ...result.data];
      songNextUrl = result.next;
    } catch (e) {
      songError = e.toString();
    } finally {
      isLoadingSongs = false;
      notifyListeners();
    }
  }

  Future<void> searchAlbums(String query) async {
    final trimmed = query.trim();
    currentAlbumQuery = trimmed;

    if (trimmed.isEmpty) {
      albumResults = [];
      albumNextUrl = null;
      albumError = null;
      notifyListeners();
      return;
    }

    isLoadingAlbums = true;
    albumError = null;
    notifyListeners();

    try {
      final result = await _provider.searchAlbums(
        query: trimmed,
        limit: 5,
        index: 0,
      );

      albumResults = result.data;
      albumNextUrl = result.next;
    } catch (e) {
      albumError = e.toString();
    } finally {
      isLoadingAlbums = false;
      notifyListeners();
    }
  }

  Future<void> loadMoreAlbums() async {
    if (albumNextUrl == null || isLoadingAlbums) return;

    isLoadingAlbums = true;
    notifyListeners();

    try {
      final result = await _provider.getAlbumsFromNextUrl(albumNextUrl!);

      albumResults = [...albumResults, ...result.data];
      albumNextUrl = result.next;
    } catch (e) {
      albumError = e.toString();
    } finally {
      isLoadingAlbums = false;
      notifyListeners();
    }
  }

  Future<DeezerAlbumDetails?> getAlbumDetails(int albumId) async {
    isLoadingAlbumDetails = true;
    albumDetailsError = null;
    notifyListeners();

    try {
      final result = await _provider.getAlbumDetails(albumId);
      return result;
    } catch (e) {
      albumDetailsError = e.toString();
      return null;
    } finally {
      isLoadingAlbumDetails = false;
      notifyListeners();
    }
  }

  void clearSongResults() {
    songResults = [];
    songNextUrl = null;
    songError = null;
    notifyListeners();
  }

  void clearAlbumResults() {
    albumResults = [];
    albumNextUrl = null;
    albumError = null;
    notifyListeners();
  }

  Future<void> loadInitialAlbums() async {
    isLoadingAlbums = true;
    albumError = null;
    notifyListeners();

    try {
      final result = await _provider.searchAlbums(
        query: 'top',
        limit: 5,
        index: 0,
      );

      albumResults = result.data;
      albumNextUrl = result.next;
    } catch (e) {
      albumError = e.toString();
    } finally {
      isLoadingAlbums = false;
      notifyListeners();
    }
  }
}