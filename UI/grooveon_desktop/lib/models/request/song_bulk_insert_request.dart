import 'package:grooveon_desktop/models/request/song_upsert_request.dart';
import 'package:json_annotation/json_annotation.dart';

part 'song_bulk_insert_request.g.dart';

@JsonSerializable(explicitToJson: true)
class SongBulkInsertRequest {
  final List<SongUpsertRequest> songs;

  SongBulkInsertRequest({
    required this.songs,
  });

  factory SongBulkInsertRequest.fromJson(Map<String, dynamic> json) =>
      _$SongBulkInsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$SongBulkInsertRequestToJson(this);
}