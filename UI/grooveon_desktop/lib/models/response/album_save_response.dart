import 'package:json_annotation/json_annotation.dart';

part 'album_save_response.g.dart';

@JsonSerializable()
class AlbumSaveResponse {
  final int? albumId;
  final bool albumCreated;
  final int savedTracksCount;
  final int existingTracksCount;

  AlbumSaveResponse({
    this.albumId,
    required this.albumCreated,
    required this.savedTracksCount,
    required this.existingTracksCount,
  });

  factory AlbumSaveResponse.fromJson(Map<String, dynamic> json) =>
      _$AlbumSaveResponseFromJson(json);

  Map<String, dynamic> toJson() => _$AlbumSaveResponseToJson(this);
}