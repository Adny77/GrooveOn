import 'package:json_annotation/json_annotation.dart';
import 'package:grooveon_mobile/models/music_search_item_response.dart';

part 'music_search_result.g.dart';

@JsonSerializable(explicitToJson: true)
class MusicSearchResult {
  final List<MusicSearchItemResponse> items;
  final int? totalCount;

  MusicSearchResult({
    this.items = const [],
    this.totalCount,
  });

  factory MusicSearchResult.fromJson(Map<String, dynamic> json) =>
      _$MusicSearchResultFromJson(json);

  Map<String, dynamic> toJson() => _$MusicSearchResultToJson(this);
}