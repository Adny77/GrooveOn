import 'package:json_annotation/json_annotation.dart';

part 'playlist_song_response.g.dart';

@JsonSerializable()
class PlaylistSongResponse {
  final int id;

  final int playlistId;
  final String? playlistName;

  final int songId;
  final String? songTitle;

  final String? artistName;
  final String? coverUrl;

  final String? externalTrackId;
  final int? durationSeconds;

  final DateTime addedAt;

  PlaylistSongResponse({
    required this.id,
    required this.playlistId,
    this.playlistName,
    required this.songId,
    this.songTitle,
    this.artistName,
    this.coverUrl,
    this.externalTrackId,
    this.durationSeconds,
    required this.addedAt,
  });

  factory PlaylistSongResponse.fromJson(Map<String, dynamic> json) =>
      _$PlaylistSongResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PlaylistSongResponseToJson(this);
}