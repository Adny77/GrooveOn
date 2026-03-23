import 'package:json_annotation/json_annotation.dart';

part 'deezer_artist.g.dart';

@JsonSerializable()
class DeezerArtist {
  final int id;
  final String name;
  final String? picture;

  @JsonKey(name: 'picture_small')
  final String? pictureSmall;

  @JsonKey(name: 'picture_medium')
  final String? pictureMedium;

  @JsonKey(name: 'picture_big')
  final String? pictureBig;

  @JsonKey(name: 'picture_xl')
  final String? pictureXl;

  DeezerArtist({
    required this.id,
    required this.name,
    this.picture,
    this.pictureSmall,
    this.pictureMedium,
    this.pictureBig,
    this.pictureXl,
  });

  factory DeezerArtist.fromJson(Map<String, dynamic> json) =>
      _$DeezerArtistFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerArtistToJson(this);
}