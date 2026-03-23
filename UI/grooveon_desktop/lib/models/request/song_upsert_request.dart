import 'package:json_annotation/json_annotation.dart';

part 'song_upsert_request.g.dart';

@JsonSerializable()
class SongUpsertRequest {
  final String externalTrackId;
  final String source;
  final String title;
  final String artistName;
  final String? albumTitle;
  final int durationSeconds;
  final String? previewUrl;
  final String? coverUrl;
  final DateTime? releaseDate;

  SongUpsertRequest({
    required this.externalTrackId,
    this.source = "Deezer",
    required this.title,
    required this.artistName,
    this.albumTitle,
    required this.durationSeconds,
    this.previewUrl,
    this.coverUrl,
    this.releaseDate,
  });

  factory SongUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$SongUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SongUpsertRequestToJson(this);
}