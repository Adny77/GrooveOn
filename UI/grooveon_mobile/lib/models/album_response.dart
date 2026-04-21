import 'package:json_annotation/json_annotation.dart';

part 'album_response.g.dart';

@JsonSerializable()
class AlbumResponse {
  final int id;
  final String? externalAlbumId;
  final String source;
  final String title;
  final int artistId;
  final String artistName;
  final DateTime? releaseDate;
  final String? coverUrl;
  final String? description;
  final DateTime createdAt;
  final int songCount;

  AlbumResponse({
    required this.id,
    this.externalAlbumId,
    required this.source,
    required this.title,
    required this.artistId,
    required this.artistName,
    this.releaseDate,
    this.coverUrl,
    this.description,
    required this.createdAt,
    required this.songCount,
  });

  factory AlbumResponse.fromJson(Map<String, dynamic> json) =>
      _$AlbumResponseFromJson(json);

  Map<String, dynamic> toJson() => _$AlbumResponseToJson(this);
}