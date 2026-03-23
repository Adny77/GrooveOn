import 'package:json_annotation/json_annotation.dart';

part 'song_duplicate_check_request.g.dart';

@JsonSerializable()
class SongDuplicateCheckRequest {
  final List<String> externalTrackIds;

  SongDuplicateCheckRequest({
    required this.externalTrackIds,
  });

  factory SongDuplicateCheckRequest.fromJson(Map<String, dynamic> json) =>
      _$SongDuplicateCheckRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SongDuplicateCheckRequestToJson(this);
}