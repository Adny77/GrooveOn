import 'package:json_annotation/json_annotation.dart';

part 'music_overview_request.g.dart';

@JsonSerializable()
class MusicOverviewRequest {
  final String mode; 
  final int userId;
  final int year;
  final int? month;
  final int take;

  MusicOverviewRequest({
    required this.mode,
    required this.userId,
    required this.year,
    this.month,
    this.take = 4,
  });

  factory MusicOverviewRequest.fromJson(Map<String, dynamic> json) =>
      _$MusicOverviewRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MusicOverviewRequestToJson(this);
}