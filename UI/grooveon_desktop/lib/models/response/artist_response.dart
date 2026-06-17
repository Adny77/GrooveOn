import 'package:json_annotation/json_annotation.dart';

part 'artist_response.g.dart';

@JsonSerializable()
class ArtistResponse {
  final int id;
  final String? externalArtistId;
  final String source;
  final String name;
  final String? picture;
  final DateTime createdAt;

  ArtistResponse({
    required this.id,
    this.externalArtistId,
    required this.source,
    required this.name,
    this.picture,
    required this.createdAt,
  });

  factory ArtistResponse.fromJson(Map<String, dynamic> json) =>
      _$ArtistResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ArtistResponseToJson(this);
}
