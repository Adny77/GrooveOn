import 'dart:async';
import 'package:flutter/material.dart';
import 'package:grooveon_mobile/deezer/provider/deezer_track_provider.dart';
import 'package:just_audio/just_audio.dart';

import '../models/player_response.dart';
import '../models/song_response.dart';
import '../utils/session.dart';
import 'base_provider.dart';

class PlayerProvider extends BaseProvider<PlayerResponse> {
  PlayerProvider() : super("Player") {
    _bindPlayerStreams();
  }

  final AudioPlayer _audioPlayer = AudioPlayer();

  SongResponse? _currentSong;
  PlayerResponse? _playerState;
  String? _currentPreviewUrl;

  bool _isVisible = false;
  bool _isLoading = false;
  bool _isPlaying = false;

  Duration _position = Duration.zero;
  Duration _duration = Duration.zero;

  StreamSubscription<Duration>? _positionSub;
  StreamSubscription<Duration?>? _durationSub;
  StreamSubscription<PlayerState>? _playerStateSub;

  SongResponse? get currentSong => _currentSong;
  PlayerResponse? get playerState => _playerState;

  bool get isVisible => _isVisible;
  bool get isLoading => _isLoading;
  bool get isPlaying => _isPlaying;
  bool get hasSong => _currentSong != null;

  Duration get position => _position;
  Duration get duration => _duration;

  String get currentTitle => _currentSong?.title ?? "";
  String get currentArtist => _currentSong?.artistName ?? "";
  String? get currentCover => _currentSong?.coverUrl;
  String? get currentPreviewUrl => _currentPreviewUrl;

  bool get canGoNext => false;
  bool get canGoPrevious => false;

  double get progress {
    if (_duration.inMilliseconds <= 0) return 0;
    return (_position.inMilliseconds / _duration.inMilliseconds).clamp(0.0, 1.0);
  }

  void _bindPlayerStreams() {
    _positionSub = _audioPlayer.positionStream.listen((value) {
      _position = value;
      notifyListeners();
    });

    _durationSub = _audioPlayer.durationStream.listen((value) {
      _duration = value ?? Duration.zero;
      notifyListeners();
    });

    _playerStateSub = _audioPlayer.playerStateStream.listen((state) async {
      _isPlaying = state.playing;

      if (state.processingState == ProcessingState.completed) {
        _position = Duration.zero;
        _isPlaying = false;
        await _syncWithBackend();
      }

      notifyListeners();
    });
  }

  Future<void> playSong(SongResponse song) async {
    if (song.externalTrackId == null || song.externalTrackId!.trim().isEmpty) {
      throw Exception("Pjesma nema externalTrackId.");
    }

    try {
      _isLoading = true;
      _isVisible = true;
      notifyListeners();

      final isSameSong = _currentSong?.id == song.id;

      if (isSameSong) {
        if (_audioPlayer.playing) {
          await _audioPlayer.pause();
        } else {
          final shouldRestart =
              _duration.inMilliseconds > 0 &&
              _position.inMilliseconds >= _duration.inMilliseconds;

          if (shouldRestart) {
            await _audioPlayer.seek(Duration.zero);
          }

          await _audioPlayer.play();
        }

        await _syncWithBackend();
        return;
      }

      final freshPreviewUrl = await DeezerTrackProvider.getPreviewUrl(
        song.externalTrackId!,
      );

      if (freshPreviewUrl == null || freshPreviewUrl.trim().isEmpty) {
        throw Exception("Preview nije dostupan za ovu pjesmu.");
      }

      await _audioPlayer.stop();

      _position = Duration.zero;
      _duration = Duration.zero;
      _currentSong = song;
      _currentPreviewUrl = freshPreviewUrl;

      await _audioPlayer.setUrl(freshPreviewUrl);
      await _audioPlayer.play();

      await _syncWithBackend();
    } catch (e) {
      debugPrint("PLAYER ERROR: $e");
      rethrow;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> togglePlayPause() async {
    if (_currentSong == null) return;

    try {
      if (_audioPlayer.playing) {
        await _audioPlayer.pause();
      } else {
        final shouldRestart =
            _duration.inMilliseconds > 0 &&
            _position.inMilliseconds >= _duration.inMilliseconds;

        if (shouldRestart) {
          await _audioPlayer.seek(Duration.zero);
        }

        await _audioPlayer.play();
      }

      await _syncWithBackend();
      notifyListeners();
    } catch (e) {
      debugPrint("TOGGLE ERROR: $e");
    }
  }

  Future<void> pause() async {
    try {
      await _audioPlayer.pause();
      await _syncWithBackend();
      notifyListeners();
    } catch (e) {
      debugPrint("PAUSE ERROR: $e");
    }
  }

  Future<void> resume() async {
    try {
      if (_currentSong == null) return;

      final shouldRestart =
          _duration.inMilliseconds > 0 &&
          _position.inMilliseconds >= _duration.inMilliseconds;

      if (shouldRestart) {
        await _audioPlayer.seek(Duration.zero);
      }

      await _audioPlayer.play();
      await _syncWithBackend();
      notifyListeners();
    } catch (e) {
      debugPrint("RESUME ERROR: $e");
    }
  }

  Future<void> seek(Duration value) async {
    try {
      await _audioPlayer.seek(value);
      await _syncWithBackend();
      notifyListeners();
    } catch (e) {
      debugPrint("SEEK ERROR: $e");
    }
  }

  Future<void> stop() async {
    try {
      await _audioPlayer.stop();
      _position = Duration.zero;
      _isPlaying = false;
      await _syncWithBackend();
      notifyListeners();
    } catch (e) {
      debugPrint("STOP ERROR: $e");
    }
  }

  Future<void> closePlayer() async {
    try {
      await _audioPlayer.stop();

      _currentSong = null;
      _currentPreviewUrl = null;
      _isVisible = false;
      _isLoading = false;
      _isPlaying = false;
      _position = Duration.zero;
      _duration = Duration.zero;

      if (_playerState != null) {
        await update(_playerState!.id, {
          "userId": Session.userId,
          "songId": _playerState!.songId,
          "currentSeconds": 0,
          "isPlaying": false,
          "isVisible": false,
        });
      }

      notifyListeners();
    } catch (e) {
      debugPrint("CLOSE PLAYER ERROR: $e");
    }
  }

  Future<void> playNext() async {
    debugPrint("playNext još nije implementiran za queue logiku.");
  }

  Future<void> playPrevious() async {
    debugPrint("playPrevious još nije implementiran za queue logiku.");
  }

  Future<void> _syncWithBackend() async {
    if (_currentSong == null || Session.userId == null) return;

    final request = {
      "userId": Session.userId,
      "songId": _currentSong!.id,
      "currentSeconds": _position.inSeconds,
      "isPlaying": _isPlaying,
      "isVisible": _isVisible,
    };

    try {
      final result = await get(filter: {"UserId": Session.userId});

      if (result.items.isEmpty) {
        final inserted = await insert(request);
        _playerState = inserted;
      } else {
        final existing = result.items.first;
        final updated = await update(existing.id, request);
        _playerState = updated;
      }
    } catch (e) {
      debugPrint("PLAYER SYNC ERROR: $e");
    }
  }

  Future<void> loadPlayerFromBackend() async {
    if (Session.userId == null) return;

    try {
      final result = await get(filter: {"UserId": Session.userId});

      if (result.items.isNotEmpty) {
        _playerState = result.items.first;
        _isVisible = _playerState!.isVisible;
        _isPlaying = _playerState!.isPlaying;
        notifyListeners();
      }
    } catch (e) {
      debugPrint("LOAD PLAYER ERROR: $e");
    }
  }

  String formatDuration(Duration value) {
    final minutes = value.inMinutes.remainder(60);
    final seconds = value.inSeconds.remainder(60);
    return "$minutes:${seconds.toString().padLeft(2, '0')}";
  }

  @override
  PlayerResponse fromJson(data) {
    return PlayerResponse.fromJson(data);
  }

  @override
  void dispose() {
    _positionSub?.cancel();
    _durationSub?.cancel();
    _playerStateSub?.cancel();
    _audioPlayer.dispose();
    super.dispose();
  }
}