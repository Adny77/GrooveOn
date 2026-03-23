import 'package:grooveon_desktop/models/response/existing_album_track_response.dart';
import 'package:json_annotation/json_annotation.dart';

part 'album_preview_response.g.dart';

@JsonSerializable(explicitToJson: true)
class AlbumPreviewResponse {
  final bool albumAlreadyExists;
  final List<ExistingAlbumTrackResponse> tracks;
  final int existingTracksCount;
  final int newTracksCount;

  AlbumPreviewResponse({
    required this.albumAlreadyExists,
    required this.tracks,
    required this.existingTracksCount,
    required this.newTracksCount,
  });

  factory AlbumPreviewResponse.fromJson(Map<String, dynamic> json) =>
      _$AlbumPreviewResponseFromJson(json);

  Map<String, dynamic> toJson() => _$AlbumPreviewResponseToJson(this);
}