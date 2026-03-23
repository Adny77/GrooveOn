import 'package:json_annotation/json_annotation.dart';

part 'song_response.g.dart';

@JsonSerializable()
class SongResponse {
  final int id;
  final String? externalTrackId;
  final String source;
  final String title;

  final int artistId;
  final String artistName;

  final int? albumId;
  final String? albumTitle;

  final int? genreId;
  final String? genreName;

  final int durationSeconds;
  final String? previewUrl;
  final String? coverUrl;
  final DateTime? releaseDate;

  final bool isActive;
  final DateTime? lastSyncedAt;
  final DateTime createdAt;

  SongResponse({
    required this.id,
    this.externalTrackId,
    required this.source,
    required this.title,
    required this.artistId,
    required this.artistName,
    this.albumId,
    this.albumTitle,
    this.genreId,
    this.genreName,
    required this.durationSeconds,
    this.previewUrl,
    this.coverUrl,
    this.releaseDate,
    required this.isActive,
    this.lastSyncedAt,
    required this.createdAt,
  });

  factory SongResponse.fromJson(Map<String, dynamic> json) =>
      _$SongResponseFromJson(json);

  Map<String, dynamic> toJson() => _$SongResponseToJson(this);
}