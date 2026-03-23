import 'package:json_annotation/json_annotation.dart';

part 'existing_album_track_response.g.dart';

@JsonSerializable()
class ExistingAlbumTrackResponse {
  final String externalTrackId;
  final String title;
  final bool alreadyExists;

  ExistingAlbumTrackResponse({
    required this.externalTrackId,
    required this.title,
    required this.alreadyExists,
  });

  factory ExistingAlbumTrackResponse.fromJson(Map<String, dynamic> json) =>
      _$ExistingAlbumTrackResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ExistingAlbumTrackResponseToJson(this);
}