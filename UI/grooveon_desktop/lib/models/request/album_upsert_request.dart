import 'package:json_annotation/json_annotation.dart';
import 'package:grooveon_desktop/models/request/song_upsert_request.dart';
import 'package:grooveon_desktop/models/request/genre_upsert_request.dart';

part 'album_upsert_request.g.dart';

@JsonSerializable(explicitToJson: true)
class AlbumUpsertRequest {
  final String externalAlbumId;
  final String? externalArtistId;
  final String source;
  final String title;
  final String artistName;
  final String? coverUrl;
  final String? description;
  final DateTime? releaseDate;

  final List<GenreUpsertRequest> genres; 

  final List<SongUpsertRequest> tracks;

  AlbumUpsertRequest({
    required this.externalAlbumId,
    this.externalArtistId,
    this.source = "Deezer",
    required this.title,
    required this.artistName,
    this.coverUrl,
    this.description,
    this.releaseDate,
    this.genres = const [],
    required this.tracks,
  });

  factory AlbumUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$AlbumUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$AlbumUpsertRequestToJson(this);
}