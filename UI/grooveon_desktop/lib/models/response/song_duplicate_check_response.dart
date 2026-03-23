import 'package:grooveon_desktop/models/response/existing_song_info_response.dart';
import 'package:json_annotation/json_annotation.dart';

part 'song_duplicate_check_response.g.dart';

@JsonSerializable(explicitToJson: true)
class SongDuplicateCheckResponse {
  final List<ExistingSongInfoResponse> existingSongs;
  final List<String> missingExternalTrackIds;

  SongDuplicateCheckResponse({
    required this.existingSongs,
    required this.missingExternalTrackIds,
  });

  factory SongDuplicateCheckResponse.fromJson(Map<String, dynamic> json) =>
      _$SongDuplicateCheckResponseFromJson(json);

  Map<String, dynamic> toJson() => _$SongDuplicateCheckResponseToJson(this);
}