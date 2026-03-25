import 'package:grooveon_desktop/models/request/genre_upsert_request.dart';
import 'package:json_annotation/json_annotation.dart';

part 'song_upsert_request.g.dart';

@JsonSerializable()
class SongUpsertRequest {
  final String externalTrackId;
  final String? externalArtistId;
  final String? externalAlbumId;
  final String source;
  final String title;
  final String artistName;
  final String? artistPicture;
  final String? albumTitle;
  final int? durationSeconds;
  final String? previewUrl;
  final String? coverUrl;
  final DateTime? releaseDate;
  final List<GenreUpsertRequest> genres;

  SongUpsertRequest({
    required this.externalTrackId,
    this.externalArtistId,
    this.externalAlbumId,
    this.source = "Deezer",
    required this.title,
    required this.artistName,
    this.artistPicture,
    this.albumTitle,
    this.durationSeconds,
    this.previewUrl,
    this.coverUrl,
    this.releaseDate,
    this.genres = const [],
  });

  factory SongUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$SongUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SongUpsertRequestToJson(this);
}