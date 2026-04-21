import 'package:json_annotation/json_annotation.dart';

part 'artist_response.g.dart';

@JsonSerializable()
class Artist {
  final int id;
  final String name;
  final String? pictureUrl;
  final String? description;

  Artist({
    required this.id,
    required this.name,
    this.pictureUrl,
    this.description,
  });

  factory Artist.fromJson(Map<String, dynamic> json) =>
      _$ArtistResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ArtistResponseToJson(this);
}