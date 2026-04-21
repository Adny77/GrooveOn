import 'package:json_annotation/json_annotation.dart';

part 'new_users_summary.g.dart';

@JsonSerializable()
class NewUsersSummary {
  final int year;
  final int month;
  final int count;

  NewUsersSummary({
    required this.year,
    required this.month,
    required this.count,
  });

  factory NewUsersSummary.fromJson(Map<String, dynamic> json) =>
      _$NewUsersSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$NewUsersSummaryToJson(this);
}