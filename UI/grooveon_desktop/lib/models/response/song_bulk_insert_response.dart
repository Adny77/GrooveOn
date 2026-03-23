import 'package:json_annotation/json_annotation.dart';

part 'song_bulk_insert_response.g.dart';

@JsonSerializable()
class SongBulkInsertResponse {
  final int savedCount;
  final List<int> savedSongIds;

  SongBulkInsertResponse({
    required this.savedCount,
    required this.savedSongIds,
  });

  factory SongBulkInsertResponse.fromJson(Map<String, dynamic> json) =>
      _$SongBulkInsertResponseFromJson(json);

  Map<String, dynamic> toJson() => _$SongBulkInsertResponseToJson(this);
}