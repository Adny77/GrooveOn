import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:just_audio/just_audio.dart';
import 'package:http/http.dart' as http;

import 'package:grooveon_mobile/config/api_config.dart';
import 'package:grooveon_mobile/helper/http_helper.dart';
import 'package:grooveon_mobile/deezer/provider/deezer_track_provider.dart';
import 'package:grooveon_mobile/models/player_song.dart';
import 'package:grooveon_mobile/models/player_response.dart';

class PlayerProvider with ChangeNotifier {
  final AudioPlayer _audioPlayer = AudioPlayer();

  PlayerSong? _currentSong;
  Map<String, dynamic>? _currentRequest;

  bool _isPlaying = false;
  bool _isLoading = false;
  bool _isCompleted = false;
  bool _isChangingSong = false;

  bool _hasNext = false;
  bool _hasPrevious = false;

  Duration _position = Duration.zero;
  Duration _duration = Duration.zero;

  Timer? _repeatTimer;

  bool get isPlaying => _isPlaying;
  bool get isLoading => _isLoading;
  bool get isCompleted => _isCompleted;

  bool get hasSong => _currentSong != null;
  PlayerSong? get currentSong => _currentSong;

  bool get hasNext => _hasNext;
  bool get hasPrevious => _hasPrevious;

  Duration get position => _position;
  Duration get duration => _duration;

  double get progress {
    if (_duration.inMilliseconds <= 0) return 0.0;
    return (_position.inMilliseconds / _duration.inMilliseconds)
        .clamp(0.0, 1.0);
  }

  PlayerProvider() {
    _audioPlayer.positionStream.listen((value) {
      _position = value;
      notifyListeners();
    });

    _audioPlayer.durationStream.listen((value) {
      _duration = value ?? Duration.zero;
      notifyListeners();

      if (_audioPlayer.playing && !_isCompleted) {
        _startRepeatTimerFromCurrentPosition();
      }
    });

    _audioPlayer.playerStateStream.listen((state) {
      if (_isChangingSong) {
        _isPlaying = state.playing;
        notifyListeners();
        return;
      }

      if (state.processingState == ProcessingState.completed) {
        _markCompleted();
        return;
      }

      if (_isCompleted) {
        _isPlaying = false;
        notifyListeners();
        return;
      }

      _isPlaying = state.playing;
      notifyListeners();
    });
  }

  Future<void> playSongWithPurpose({
    required Map<String, dynamic> request,
  }) async {
    try {
      _repeatTimer?.cancel();
      _isLoading = true;
      _isCompleted = false;
      notifyListeners();

      final result = await _callPlay(request);

      if (result.externalTrackId == null ||
          result.externalTrackId!.trim().isEmpty) {
        throw Exception("ExternalTrackId is not available.");
      }

      await _loadAndPlay(result, request);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      debugPrint("PLAY ERROR: $e");
      _repeatTimer?.cancel();
      _isLoading = false;
      _isChangingSong = false;
      notifyListeners();
      rethrow;
    }
  }

  Future<void> playNext() async {
    try {
      if (!_hasNext || _currentSong == null || _currentRequest == null) return;

      _repeatTimer?.cancel();
      _isLoading = true;
      _isCompleted = false;
      notifyListeners();

      final request = {
        ..._currentRequest!,
        "songId": _currentSong!.id,
      };

      final result = await _callNext(request);

      if (result.externalTrackId == null ||
          result.externalTrackId!.trim().isEmpty) {
        throw Exception("ExternalTrackId is not available.");
      }

      await _loadAndPlay(result, request);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      debugPrint("NEXT ERROR: $e");
      _repeatTimer?.cancel();
      _isLoading = false;
      _isChangingSong = false;
      notifyListeners();
    }
  }

  Future<void> playPrevious() async {
    try {
      if (!_hasPrevious || _currentSong == null || _currentRequest == null) {
        return;
      }

      _repeatTimer?.cancel();
      _isLoading = true;
      _isCompleted = false;
      notifyListeners();

      final request = {
        ..._currentRequest!,
        "songId": _currentSong!.id,
      };

      final result = await _callPrevious(request);

      if (result.externalTrackId == null ||
          result.externalTrackId!.trim().isEmpty) {
        throw Exception("ExternalTrackId is not available.");
      }

      await _loadAndPlay(result, request);

      _isLoading = false;
      notifyListeners();
    } catch (e) {
      debugPrint("PREVIOUS ERROR: $e");
      _repeatTimer?.cancel();
      _isLoading = false;
      _isChangingSong = false;
      notifyListeners();
    }
  }

  Future<void> pause() async {
    await _audioPlayer.pause();

    _repeatTimer?.cancel();

    _isPlaying = false;
    notifyListeners();
  }

  Future<void> resume() async {
    if (_currentSong == null) return;

    if (_isCompleted) {
      await repeatCurrentSong();
      return;
    }

    await _audioPlayer.play();

    _isPlaying = true;
    _isCompleted = false;

    _startRepeatTimerFromCurrentPosition();

    notifyListeners();
  }

  Future<void> togglePlayPause() async {
    if (_audioPlayer.playing) {
      await pause();
    } else {
      await resume();
    }
  }

  Future<void> repeatCurrentSong() async {
    if (_currentSong == null) return;

    _repeatTimer?.cancel();
    _isChangingSong = true;

    await _audioPlayer.seek(Duration.zero);
    await _audioPlayer.play();

    _isChangingSong = false;

    _position = Duration.zero;
    _isPlaying = true;
    _isCompleted = false;

    _startRepeatTimerFromCurrentPosition();

    notifyListeners();
  }

  Future<void> stopPlayer() async {
  try {
    _repeatTimer?.cancel();

    await _audioPlayer.stop();

    _currentSong = null;
    _currentRequest = null;

    _isPlaying = false;
    _isLoading = false;
    _isCompleted = false;
    _isChangingSong = false;

    _hasNext = false;
    _hasPrevious = false;

    _position = Duration.zero;
    _duration = Duration.zero;

    notifyListeners();
  } catch (e) {
    debugPrint("STOP ERROR: $e");
  }
}

  Future<void> seek(Duration position) async {
    await _audioPlayer.seek(position);
    _position = position;

    if (_isCompleted && position < _duration) {
      _isCompleted = false;
    }

    if (_audioPlayer.playing) {
      _startRepeatTimerFromCurrentPosition();
    } else {
      _repeatTimer?.cancel();
    }

    notifyListeners();
  }

  Future<void> _loadAndPlay(
    PlayerResponse result,
    Map<String, dynamic> request,
  ) async {
    final previewUrl = await _resolvePreviewUrl(result);

    if (previewUrl == null || previewUrl.trim().isEmpty) {
      throw Exception("Preview is not available.");
    }

    _setCurrentSongFromResponse(result, request, previewUrl);

    _repeatTimer?.cancel();
    _isChangingSong = true;

    await _audioPlayer.stop();
    await _audioPlayer.setUrl(previewUrl);
    await _audioPlayer.play();

    _isChangingSong = false;

    _isPlaying = true;
    _isCompleted = false;

    _startRepeatTimerFromCurrentPosition();

    notifyListeners();
  }

  Future<String?> _resolvePreviewUrl(PlayerResponse result) async {
    if (result.previewUrl != null && result.previewUrl!.trim().isNotEmpty) {
      return result.previewUrl;
    }

    return DeezerTrackProvider.getPreviewUrl(result.externalTrackId);
  }

  void _startRepeatTimerFromCurrentPosition() {
    _repeatTimer?.cancel();

    if (_duration.inMilliseconds <= 0) return;

    final remaining = _duration - _position;

    if (remaining.inMilliseconds <= 0) {
      _markCompleted();
      return;
    }

    _repeatTimer = Timer(remaining, _markCompleted);
  }

  void _markCompleted() {
    _repeatTimer?.cancel();

    _isPlaying = false;
    _isCompleted = true;

    notifyListeners();
  }

  void _setCurrentSongFromResponse(
    PlayerResponse result,
    Map<String, dynamic> request,
    String previewUrl,
  ) {
    _currentSong = PlayerSong(
      id: result.songId,
      title: result.title,
      artistName: result.artistName,
      duration: result.duration,
      coverUrl: result.coverUrl,
      previewUrl: previewUrl,
      externalTrackId: result.externalTrackId,
    );

    _currentRequest = {
      "userId": request["userId"],
      "songId": result.songId,
      "purpose": request["purpose"],
      "artistId": request["artistId"],
      "playlistId": request["playlistId"],
    };

    _hasNext = result.hasNext;
    _hasPrevious = result.hasPrevious;

    _position = Duration.zero;
    _duration = Duration.zero;
  }

  String formatDuration(Duration value) {
    final minutes = value.inMinutes.remainder(60);
    final seconds = value.inSeconds.remainder(60);
    return "$minutes:${seconds.toString().padLeft(2, '0')}";
  }

  Future<PlayerResponse> _callPlay(Map<String, dynamic> request) async {
    final url = Uri.parse('${ApiConfig.apiBase}/api/player/random/play');

    final response = await http.post(
      url,
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);
    return PlayerResponse.fromJson(jsonDecode(response.body));
  }

  Future<PlayerResponse> _callNext(Map<String, dynamic> request) async {
    final url = Uri.parse('${ApiConfig.apiBase}/api/player/random/next');

    final response = await http.post(
      url,
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);
    return PlayerResponse.fromJson(jsonDecode(response.body));
  }

  Future<PlayerResponse> _callPrevious(Map<String, dynamic> request) async {
    final url = Uri.parse('${ApiConfig.apiBase}/api/player/random/previous');

    final response = await http.post(
      url,
      headers: HttpHelper.getHeaders(),
      body: jsonEncode(request),
    );

    HttpHelper.checkResponse(response);
    return PlayerResponse.fromJson(jsonDecode(response.body));
  }

  @override
  void dispose() {
    _repeatTimer?.cancel();
    _audioPlayer.dispose();
    super.dispose();
  }
}
